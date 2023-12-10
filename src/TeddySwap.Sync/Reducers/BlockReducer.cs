using Microsoft.EntityFrameworkCore;
using PallasDotnet.Models;
using TeddySwap.Data;
using BlockEntity = TeddySwap.Data.Models.Block;
namespace TeddySwap.Sync.Reducers;

public class BlockReducer(IDbContextFactory<TeddySwapDbContext> dbContextFactory, ILogger<BlockReducer> logger) : IBlockReducer
{
    private TeddySwapDbContext _dbContext = default!;
    private readonly ILogger<BlockReducer> _logger = logger;

    public async Task RollBackwardAsync(NextResponse response)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        _dbContext.Blocks.RemoveRange(_dbContext.Blocks.Where(b => b.Slot > response.Block.Slot));
        await _dbContext.SaveChangesAsync();
        _dbContext.Dispose();
    }

    public async Task RollForwardAsync(NextResponse response)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        _dbContext.Blocks.Add(new BlockEntity(
            response.Block.Hash.ToHex(),
            response.Block.Number,
            response.Block.Slot
        ));

        await _dbContext.SaveChangesAsync();
        _dbContext.Dispose();
    }
}