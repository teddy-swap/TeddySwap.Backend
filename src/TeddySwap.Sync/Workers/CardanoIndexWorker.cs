using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PallasDotnet;
using PallasDotnet.Models;
using TeddySwap.Data;
namespace TeddySwap.Sync.Workers;

public class CardanoIndexWorker(IConfiguration configuration, ILogger<CardanoIndexWorker> logger, IDbContextFactory<TeddySwapDbContext> dbContextFactory) : BackgroundService
{
    private readonly NodeClient _nodeClient = new();
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<CardanoIndexWorker> _logger = logger;
    private readonly IDbContextFactory<TeddySwapDbContext> _dbContextFactory = dbContextFactory;
    private TeddySwapDbContext DbContext { get; set; } = null!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        DbContext = _dbContextFactory.CreateDbContext();

        var tip = await _nodeClient.ConnectAsync(_configuration.GetValue<string>("CardanoNodeSocketPath")!, _configuration.GetValue<ulong>("CardanoNetworkMagic"));
        _logger.Log(LogLevel.Information, "Connected to Cardano Node: {Tip}", tip);

        await _nodeClient.StartChainSyncAsync(new Point(
            _configuration.GetValue<ulong>("CardanoIndexStartSlot"),
            Hash.FromHex(_configuration.GetValue<string>("CardanoIndexStartHash")!)
        ));

        await foreach (var response in GetChainSyncResponsesAsync(stoppingToken))
        {
            if (response.Action == NextResponseAction.RollForward)
            {
                _logger.Log(
                    LogLevel.Information, "New Chain Event {Action}: {BlockNumber} Slot: {Slot}",
                    response.Action,
                    response.Block.Number,
                    response.Block.Slot
                );
                DbContext.Blocks.Add(new(response.Block.Hash.ToHex(), response.Block.Number, response.Block.Slot));
                DbContext.SaveChanges();
            }
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
