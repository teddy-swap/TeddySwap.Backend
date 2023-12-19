using Microsoft.EntityFrameworkCore;
using TeddySwap.Data.Models;
using TeddySwap.Data.Models.Reducers;

namespace TeddySwap.Data.Services;

public class YieldFarmingDataService(IDbContextFactory<TeddySwapDbContext> dbContextFactory)
{
    private readonly IDbContextFactory<TeddySwapDbContext> _dbContextFactory = dbContextFactory;

    public async Task<IEnumerable<LiquidityByAddressItem>> GetAllLiquidityAsync()
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        return (await dbContext.LiquidityByAddress
            .GroupBy(item => item.Address)
            .Select(group => group.OrderByDescending(item => item.Slot).FirstOrDefault())
            .ToListAsync()
        ) as IEnumerable<LiquidityByAddressItem> ?? [];
    }

    public async Task<LiquidityByAddressItem> GetLatestLiquidityByAddressAsync(string address)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        return (await dbContext.LiquidityByAddress
            .Where(l => l.Address == address)
            .OrderByDescending(l => l.Slot)
            .FirstOrDefaultAsync())!;
    }

    public async Task<IEnumerable<YieldRewardByAddress>> YieldRewardByAddressAsync(string address)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        return (await dbContext.YieldRewardByAddress
            .Where(l => l.Address == address)
            .OrderByDescending(l => l.Slot)
            .ToListAsync()
        ) as IEnumerable<YieldRewardByAddress> ?? [];
    }

    public async Task<IEnumerable<YieldRewardByAddress>> UnclaimedYieldRewardByAddressAsync(string address)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        return (await dbContext.YieldRewardByAddress
            .Where(l => l.Address == address && l.IsClaimed == false)
            .OrderByDescending(l => l.Slot)
            .ToListAsync()
        ) as IEnumerable<YieldRewardByAddress> ?? [];
    }

    public async Task<IEnumerable<YieldRewardByAddress>> YieldRewardByAddressSinceDaysAgoAsync(string address, int daysAgo)
    {
        var sinceDate = DateTimeOffset.UtcNow.AddDays(-daysAgo);

        await using var dbContext = _dbContextFactory.CreateDbContext();
        return (await dbContext.YieldRewardByAddress
            .Where(l => l.Address == address && l.Timestamp >= sinceDate)
            .OrderByDescending(l => l.Slot)
            .ToListAsync()
        ) as IEnumerable<YieldRewardByAddress> ?? Array.Empty<YieldRewardByAddress>();
    }


    public async Task<IEnumerable<YieldRewardByAddress>> YieldRewardByAddressAsync(string address, int limit, int offset)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        return (await dbContext.YieldRewardByAddress
            .Where(l => l.Address == address)
            .OrderByDescending(l => l.Slot)
            .Skip(offset)
            .Take(limit)
            .ToListAsync()
        ) as IEnumerable<YieldRewardByAddress> ?? [];
    }

    public async Task<IEnumerable<YieldRewardByAddress>> YieldRewardByAddressAsync(string address, ulong sinceSlot)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        return (await dbContext.YieldRewardByAddress
            .Where(l => l.Address == address && l.Slot > sinceSlot)
            .OrderByDescending(l => l.Slot)
            .ToListAsync()
        ) as IEnumerable<YieldRewardByAddress> ?? [];
    }

    public async Task<ulong> TotalUnclaimedRewardsAsync(string address)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        return (ulong)(await dbContext.YieldRewardByAddress
            .Where(l => l.Address == address && l.IsClaimed == false)
            .SumAsync(l => (double)l.Amount));
    }

    public async Task<IEnumerable<YieldFarmingDistribution>> YieldRewardDistributionAsync()
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.YieldRewardByAddress
            .GroupBy(item => item.BlockNumber)
            .Select(group => new YieldFarmingDistribution
            {
                BlockNumber = group.Key,
                Amount = (ulong)group.Sum(item => (decimal)item.Amount),
                Slot = group.Max(item => item.Slot)
            }).ToListAsync();
    }

    public async Task<IEnumerable<YieldFarmingDistribution>> YieldRewardDistributionSinceDaysAgoAsync(int daysAgo)
    {
        var sinceDate = DateTime.UtcNow.AddDays(-daysAgo);

        await using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.YieldRewardByAddress
            .Where(item => item.Timestamp >= sinceDate)
            .GroupBy(item => item.BlockNumber)
            .Select(group => new YieldFarmingDistribution
            {
                BlockNumber = group.Key,
                Amount = (ulong)group.Sum(item => (decimal)item.Amount),
                Bonus = (ulong)group.Sum(item => (decimal)item.Bonus),
                Slot = group.Max(item => item.Slot)
            }).ToListAsync();
    }

    public async Task<IEnumerable<YieldFarmingDistribution>> ClaimedYieldRewardDistributionSinceDaysAgoAsync(int daysAgo)
    {
        var sinceDate = DateTime.UtcNow.AddDays(-daysAgo);

        await using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.YieldRewardByAddress
            .Where(item => item.Timestamp >= sinceDate)
            .Where(item => item.IsClaimed == true)
            .GroupBy(item => item.BlockNumber)
            .Select(group => new YieldFarmingDistribution
            {
                BlockNumber = group.Key,
                Amount = (ulong)group.Sum(item => (decimal)item.Amount),
                Bonus = (ulong)group.Sum(item => (decimal)item.Bonus),
                Slot = group.Max(item => item.Slot)
            }).ToListAsync();
    }

    public async Task<IEnumerable<YieldClaimRequest>> GetPendingYieldClaimRequestsAsync()
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        var requests = await dbContext.YieldClaimRequests
            .Where(r => r.ProcessTxHash == null)
            .ToListAsync();

        // Return Unique requests by Address
        return requests
            .OrderByDescending(r => r.TBCs.Length)
            .GroupBy(r => r.Address)
            .Select(g => g.First());
    }

    public async Task<IEnumerable<string>> GetClaimedTbcLastDayAsync(ulong currentSlot)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        var dayInSecs = (ulong)(24 * 60 * 60);
        var lastDayInSlot = currentSlot - dayInSecs;

        return (await dbContext.YieldClaimRequests
            .Where(r => r.Slot > lastDayInSlot && r.ProcessTxHash != null)
            .ToListAsync())
            .SelectMany(r => r.TBCs)
            .Distinct();
    }

    public async Task SetYieldRewardByAddressClaimedAsync(IEnumerable<YieldRewardByAddress> yieldRewardByAddresses, string txHash)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();

        foreach (var yieldRewardByAddress in yieldRewardByAddresses)
        {
            yieldRewardByAddress.IsClaimed = true;
            yieldRewardByAddress.ClaimTxId = txHash;

            // Attach the entity to the DbContext if it's not already tracked
            dbContext.YieldRewardByAddress.Attach(yieldRewardByAddress);

            // Explicitly mark the entity as modified
            dbContext.Entry(yieldRewardByAddress).State = EntityState.Modified;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SetYieldClaimRequestsProcessedAsync(IEnumerable<YieldClaimRequest> yieldClaimRequests, string txHash, ulong blockNumber, ulong Slot)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();

        foreach (var yieldClaimRequest in yieldClaimRequests)
        {
            yieldClaimRequest.ProcessTxHash = txHash;
            yieldClaimRequest.ProcessBlockNumber = blockNumber;
            yieldClaimRequest.ProcessSlot = Slot;

            // Attach the entity to the DbContext if it's not already tracked
            dbContext.YieldClaimRequests.Attach(yieldClaimRequest);

            // Explicitly mark the entity as modified
            dbContext.Entry(yieldClaimRequest).State = EntityState.Modified;
        }

        await dbContext.SaveChangesAsync();
    }

}
