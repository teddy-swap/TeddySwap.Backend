using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.TxOutput)]
[DbContext(DbContextVariant.Core)]
public class TxOutputReducer : OuraReducerBase, IOuraCoreReducer
{
    public async Task ReduceAsync(OuraTxOutput txOutput, TeddySwapSinkCoreDbContext _dbContext)
    {
        if (txOutput is not null &&
            txOutput.OutputIndex is not null &&
            txOutput.Amount is not null &&
            txOutput.Address is not null &&
            txOutput.TxHash is not null &&
            txOutput.Context is not null &&
            txOutput.Context.BlockHash is not null)
        {
            TxOutput newTxOutput = new()
            {
                Amount = (ulong)txOutput.Amount,
                Address = txOutput.Address,
                Index = (ulong)txOutput.OutputIndex,
                DatumCbor = txOutput.DatumCbor,
                TxHash = txOutput.TxHash,
                BlockHash = txOutput.Context.BlockHash
            };

            await _dbContext.TxOutputs.AddAsync(newTxOutput);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RollbackAsync(Block rollbackBlock, TeddySwapSinkCoreDbContext _dbContext)
    {
        List<TxOutput> outputs = await _dbContext.TxOutputs
            .Where(o => o.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.TxOutputs.RemoveRange(outputs);
        await _dbContext.SaveChangesAsync();
    }
}