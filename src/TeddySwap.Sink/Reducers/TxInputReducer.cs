using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.TxInput)]
[DbContext(DbContextVariant.Core)]
public class TxInputReducer : OuraReducerBase, IOuraCoreReducer
{
    public async Task ReduceAsync(OuraTxInput txInput, TeddySwapSinkCoreDbContext _dbContext)
    {
        if (txInput is not null &&
            txInput.TxHash is not null &&
            txInput.Context is not null &&
            txInput.Context.TxHash is not null &&
            txInput.Context.BlockHash is not null)
        {
            await _dbContext.TxInputs.AddAsync(new()
            {
                TxHash = txInput.Context.TxHash,
                TxOutputHash = txInput.TxHash,
                TxOutputIndex = txInput.Index,
                BlockHash = txInput.Context.BlockHash
            });

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RollbackAsync(Block rollbackBlock, TeddySwapSinkCoreDbContext _dbContext)
    {
        var inputs = await _dbContext.TxInputs
            .Where(i => i.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.TxInputs.RemoveRange(inputs);
        await _dbContext.SaveChangesAsync();
    }
}