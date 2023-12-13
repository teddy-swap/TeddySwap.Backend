using Microsoft.EntityFrameworkCore;
using TeddySwap.Data.Models;
using TeddySwap.Data.Models.Reducers;

namespace TeddySwap.Data.Services;

public class BlockDataService(IDbContextFactory<TeddySwapDbContext> dbContextFactory)
{
    private readonly IDbContextFactory<TeddySwapDbContext> _dbContextFactory = dbContextFactory;

    public async Task<Block> GetLatestBlockAsync()
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        return (await dbContext.Blocks
            .OrderByDescending(b => b.Number)
            .FirstOrDefaultAsync())!;
    }
}