using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.TxOutput)]
public class TxOutputReducer : OuraReducerBase, IOuraCoreReducer
{
    private readonly ILogger<TxOutputReducer> _logger;
    private IDbContextFactory<TeddySwapSinkCoreDbContext> _dbContextFactory;
    public TxOutputReducer(
        ILogger<TxOutputReducer> logger,
        IDbContextFactory<TeddySwapSinkCoreDbContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task ReduceAsync(OuraTxOutput txOutput)
    {
        if (txOutput is not null &&
            txOutput.Context is not null &&
            txOutput.OutputIndex is not null &&
            txOutput.Context.BlockHash is not null &&
            txOutput.Amount is not null &&
            txOutput.Address is not null &&
            txOutput.TxHash is not null)
        {
            using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();
            Transaction? tx = await _dbContext.Transactions
                .Include(tx => tx.Block)
                .Where(tx => tx.Hash == txOutput.TxHash)
                .FirstOrDefaultAsync();

            if (tx is not null)
            {
                TxOutput? existingOutput = await _dbContext.TxOutputs
                    .Where(o => o.TxHash == tx.Hash && o.Index == txOutput.OutputIndex)
                    .FirstOrDefaultAsync();

                if (existingOutput is not null) return;

                TxOutput newTxOutput = new()
                {
                    Amount = (ulong)txOutput.Amount,
                    Address = txOutput.Address,
                    Index = (ulong)txOutput.OutputIndex,
                    DatumCbor = txOutput.DatumCbor,
                    TxHash = txOutput.TxHash,
                    Transaction = tx
                };

                await _dbContext.TxOutputs.AddAsync(newTxOutput);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    public async Task RollbackAsync(Block _) => await Task.CompletedTask;
}