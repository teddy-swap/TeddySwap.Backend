
using Microsoft.EntityFrameworkCore;
using TeddySwap.Data;
using TeddySwap.Data.Services;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Workers;

public class CardanoWorker(
    IDbContextFactory<TeddySwapDbContext> dbContextFactory,
    ILogger<CardanoWorker> logger,
    BlockDataService blockDataService,
    CardanoDataService cardanoDataService
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var dbContext = await dbContextFactory.CreateDbContextAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            var latestBlock = await blockDataService.GetLatestBlockAsync();

            if (latestBlock.Number > cardanoDataService.CurrentBlockNumber)
            {
                logger.LogInformation("New block: {BlockNumber}", latestBlock.Number);
                cardanoDataService.CurrentBlockNumber = latestBlock.Number;
                cardanoDataService.TriggerHeartbeat(new HeartbeatEventArgs { BlockNumber = latestBlock.Number });
            }

            await Task.Delay(10000, stoppingToken);
        }
    }
}