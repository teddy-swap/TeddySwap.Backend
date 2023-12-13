using Microsoft.EntityFrameworkCore;
using TeddySwap.Data.Models.Reducers;
using TeddySwap.Data.Services;

namespace TeddySwap.Data;

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
}
