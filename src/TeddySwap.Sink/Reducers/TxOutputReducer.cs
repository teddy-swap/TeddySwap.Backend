using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.TxOutput)]
public class TxOutputReducer : OuraReducerBase, IOuraCoreReducer
{
    private readonly IDbContextFactory<TeddySwapSinkCoreDbContext> _dbContextFactory;
    public TxOutputReducer(IDbContextFactory<TeddySwapSinkCoreDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task ReduceAsync(OuraTxOutput txOutput)
    {
        if (txOutput is not null &&
            txOutput.OutputIndex is not null &&
            txOutput.Amount is not null &&
            txOutput.Address is not null &&
            txOutput.TxHash is not null &&
            txOutput.Context is not null &&
            txOutput.Context.BlockHash is not null)
        {
            using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

            TxOutput newTxOutput = new()
            {
                Amount = (ulong)txOutput.Amount,
                Address = txOutput.Address,
                Index = (ulong)txOutput.OutputIndex,
                DatumCbor = txOutput.DatumCbor,
                TxHash = txOutput.TxHash,
                Blockhash = txOutput.Context.BlockHash
            };

            await _dbContext.TxOutputs.AddAsync(newTxOutput);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RollbackAsync(Block _)
    {
        // @TODO: Implement rollback
    }
}