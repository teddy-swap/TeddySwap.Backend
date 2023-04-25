using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.TxInput)]
public class TxInputReducer : OuraReducerBase, IOuraCoreReducer
{
    private readonly ILogger<TxInputReducer> _logger;
    private readonly IDbContextFactory<TeddySwapSinkCoreDbContext> _dbContextFactory;
    public TxInputReducer(
        ILogger<TxInputReducer> logger,
        IDbContextFactory<TeddySwapSinkCoreDbContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task ReduceAsync(OuraTxInput txInput)
    {
        if (txInput is not null &&
            txInput.TxHash is not null &&
            txInput.Context is not null &&
            txInput.Context.TxHash is not null &&
            txInput.Context.BlockHash is not null)
        {
            using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

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

    public async Task RollbackAsync(Block rollbackBlock)
    {
        using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var inputs = await _dbContext.TxInputs
            .Where(i => i.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.TxInputs.RemoveRange(inputs);
        await _dbContext.SaveChangesAsync();
    }
}