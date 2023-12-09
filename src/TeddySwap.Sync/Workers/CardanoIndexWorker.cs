using System.ComponentModel;
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
    IEnumerable<ICoreReducer> coreReducers,
    IEnumerable<IReducer> reducers
) : BackgroundService
{
    private readonly NodeClient _nodeClient = new();
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<CardanoIndexWorker> _logger = logger;
    private readonly IDbContextFactory<TeddySwapDbContext> _dbContextFactory = dbContextFactory;
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

        await foreach (var response in GetChainSyncResponsesAsync(stoppingToken))
        {
            _logger.Log(
                LogLevel.Information, "New Chain Event {Action}: {Slot}",
                response.Action,
                response.Block.Slot
            );

            var actionMethodMap = new Dictionary<NextResponseAction, Func<IReducer, NextResponse, Task>>
            {
                { NextResponseAction.RollForward, (reducer, response) => reducer.RollForwardAsync(response) },
                { NextResponseAction.RollBack, (reducer, response) => reducer.RollBackwardAsync(response) }
            };

            var reducerAction = actionMethodMap[response.Action];

            await Task.WhenAll(_coreReducers.Select(reducer => reducerAction(reducer, response)));
            await Task.WhenAll(_reducers.Select(reducer => reducerAction(reducer, response)));
        }

        await _nodeClient.DisconnectAsync();
    }


    private async IAsyncEnumerable<NextResponse> GetChainSyncResponsesAsync([EnumeratorCancellation] CancellationToken stoppingToken)
    {
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        var responseReady = new TaskCompletionSource<NextResponse>();

        void Handler(object? sender, ChainSyncNextResponseEventArgs e)
        {
            responseReady.TrySetResult(e.NextResponse);
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
                yield return await responseReady.Task.WaitAsync(stoppingToken);
                responseReady = new TaskCompletionSource<NextResponse>();
            }
        }
        finally
        {
            _nodeClient.ChainSyncNextResponse -= Handler;
            _nodeClient.Disconnected -= DisconnectedHandler;
        }
    }
}
