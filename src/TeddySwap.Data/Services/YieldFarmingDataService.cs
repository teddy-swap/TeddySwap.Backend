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
}
