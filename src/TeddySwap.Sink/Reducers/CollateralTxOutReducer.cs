using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.CollateralOutput)]
[DbContext(DbContextVariant.Core)]
public class CollateralTxOutReducer : OuraReducerBase
{
    public async Task ReduceAsync(OuraCollateralOutput collateralOutput, TeddySwapSinkCoreDbContext _dbContext)
    {
        if (collateralOutput is not null &&
            collateralOutput.Address is not null &&
            collateralOutput.Context is not null &&
            collateralOutput.Context.BlockHash is not null &&
            collateralOutput.Context.TxHash is not null)
        {
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

    public async Task RollbackAsync(Block rollbackBlock, TeddySwapSinkCoreDbContext _dbContext)
    {
        var outputs = await _dbContext.TxOutputs
            .Where(o => o.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.TxOutputs.RemoveRange(outputs);
        await _dbContext.SaveChangesAsync();
    }
}