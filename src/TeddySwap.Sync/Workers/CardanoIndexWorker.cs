using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PallasDotnet;
using PallasDotnet.Models;
using TeddySwap.Data;
using TeddySwap.Sync.Reducers;
namespace TeddySwap.Sync.Workers;

public class CardanoIndexWorker(
    IConfiguration configuration,
    ILogger<CardanoIndexWorker> logger,
    IDbContextFactory<TeddySwapDbContext> dbContextFactory,
    IBlockReducer blockReducer,
    IEnumerable<ICoreReducer> coreReducers,
    IEnumerable<IReducer> reducers
) : BackgroundService
{
    private readonly NodeClient _nodeClient = new();
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<CardanoIndexWorker> _logger = logger;
    private readonly IDbContextFactory<TeddySwapDbContext> _dbContextFactory = dbContextFactory;
    private readonly IBlockReducer _blockReducer = blockReducer;
    private readonly IEnumerable<ICoreReducer> _coreReducers = coreReducers;
    private readonly IEnumerable<IReducer> _reducers = reducers;
    private TeddySwapDbContext DbContext { get; set; } = null!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        DbContext = _dbContextFactory.CreateDbContext();

        DbContext.Blocks.OrderByDescending(b => b.Slot).Take(1).ToList().ForEach(block =>
        {
            _configuration["CardanoIndexStartSlot"] = block.Slot.ToString();
            _configuration["CardanoIndexStartHash"] = block.Id;
        });

        var tip = await _nodeClient.ConnectAsync(_configuration.GetValue<string>("CardanoNodeSocketPath")!, _configuration.GetValue<ulong>("CardanoNetworkMagic"));
        _logger.Log(LogLevel.Information, "Connected to Cardano Node: {Tip}", tip);

        await _nodeClient.StartChainSyncAsync(new Point(
            _configuration.GetValue<ulong>("CardanoIndexStartSlot"),
            Hash.FromHex(_configuration.GetValue<string>("CardanoIndexStartHash")!)
        ));

        await GetChainSyncResponsesAsync(stoppingToken);
        await _nodeClient.DisconnectAsync();
    }


    private async Task GetChainSyncResponsesAsync(CancellationToken stoppingToken)
    {
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        void Handler(object? sender, ChainSyncNextResponseEventArgs e)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = e.NextResponse;
            _logger.Log(
                LogLevel.Information, "New Chain Event {Action}: {Slot} Block: {Block}",
                response.Action,
                response.Block.Slot,
                response.Block.Number
            );

            var actionMethodMap = new Dictionary<NextResponseAction, Func<IReducer, NextResponse, Task>>
            {
                { NextResponseAction.RollForward, (reducer, response) => reducer.RollForwardAsync(response) },
                { NextResponseAction.RollBack, (reducer, response) => reducer.RollBackwardAsync(response) }
            };

            var reducerAction = actionMethodMap[response.Action];

            Task.WhenAll(_coreReducers.Select(reducer => reducerAction(reducer, response))).Wait(stoppingToken);
            Task.WhenAll(_reducers.Select(reducer => reducerAction(reducer, response))).Wait(stoppingToken);
            reducerAction(_blockReducer, response).Wait(stoppingToken);

            stopwatch.Stop();

            _logger.Log(
                LogLevel.Information,
                "Processed Chain Event {Action}: {Slot} Block: {Block} in {ElapsedMilliseconds} ms",
                response.Action,
                response.Block.Slot,
                response.Block.Number,
                stopwatch.ElapsedMilliseconds
            );
        }

        void DisconnectedHandler(object? sender, EventArgs e)
        {
            linkedCts.Cancel();
        }

        _nodeClient.ChainSyncNextResponse += Handler;
        _nodeClient.Disconnected += DisconnectedHandler;

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100, stoppingToken);
            }
        }
        finally
        {
            _nodeClient.ChainSyncNextResponse -= Handler;
            _nodeClient.Disconnected -= DisconnectedHandler;
        }
    }
}
