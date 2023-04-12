using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.CollateralOutput)]
public class CollateralTxOutReducer : OuraReducerBase
{
    private readonly IDbContextFactory<TeddySwapSinkCoreDbContext> _dbContextFactory;
    public CollateralTxOutReducer(IDbContextFactory<TeddySwapSinkCoreDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task ReduceAsync(OuraCollateralOutput collateralOutput)
    {
        if (collateralOutput is not null &&
            collateralOutput.Address is not null &&
            collateralOutput.Context is not null &&
            collateralOutput.Context.BlockHash is not null &&
            collateralOutput.Context.TxHash is not null)
        {
            using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

            await _dbContext.TxOutputs.AddAsync(new()
            {
                Address = collateralOutput.Address,
                Amount = collateralOutput.Amount,
                TxHash = collateralOutput.Context.TxHash,
                BlockHash = collateralOutput.Context.BlockHash
            });
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RollbackAsync(Block rollbackBlock)
    {
        using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var outputs = await _dbContext.TxOutputs
            .Where(o => o.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.TxOutputs.RemoveRange(outputs);
        await _dbContext.SaveChangesAsync();
    }
}