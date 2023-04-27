using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.CollateralInput)]
[DbContext(DbContextVariant.Core)]
public class CollateralTxInReducer : OuraReducerBase
{
    public async Task ReduceAsync(OuraTxInput txInput, TeddySwapSinkCoreDbContext _dbContext)
    {
        if (txInput is not null &&
            txInput.TxHash is not null &&
            txInput.Context is not null &&
            txInput.Context.TxHash is not null &&
            txInput.Context.BlockHash is not null)
        {
            await _dbContext.CollateralTxIns.AddAsync(new()
            {
                TxHash = txInput.TxHash,
                TxOutputHash = txInput.TxHash,
                TxOutputIndex = txInput.Index,
                BlockHash = txInput.Context.BlockHash
            });

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RollbackAsync(Block rollbackBlock, TeddySwapSinkCoreDbContext _dbContext)
    {
        var inputs = await _dbContext.CollateralTxIns
            .Where(i => i.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.CollateralTxIns.RemoveRange(inputs);
        await _dbContext.SaveChangesAsync();
    }
}