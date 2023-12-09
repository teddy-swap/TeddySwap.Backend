using Microsoft.EntityFrameworkCore;
using PallasDotnet.Models;
using TeddySwap.Data;
using TransactionOutputEntity = TeddySwap.Data.Models.TransactionOutput;
using ValueEntity = TeddySwap.Data.Models.Value;

namespace TeddySwap.Sync.Reducers;

public class TransactionOutputReducer(IDbContextFactory<TeddySwapDbContext> dbContextFactory, ILogger<LovelaceByAddressReducer> logger) : ICoreReducer
{
    private readonly TeddySwapDbContext _dbContext = dbContextFactory.CreateDbContext();
    private readonly ILogger<LovelaceByAddressReducer> _logger = logger;

    public async Task RollForwardAsync(NextResponse response)
    {
        response.Block.TransactionBodies.ToList().ForEach(txBody =>
        {
            txBody.Outputs.ToList().ForEach(output =>
            {
                _dbContext.TransactionOutputs.Add(MapTransactionOutput(txBody.Id.ToHex(), response.Block.Slot, output));
            });
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task RollBackwardAsync(NextResponse response)
    {
        var rollbackSlot = response.Block.Slot;
        _dbContext.TransactionOutputs.RemoveRange(_dbContext.TransactionOutputs.Where(tx => tx.Slot > rollbackSlot));
        await _dbContext.SaveChangesAsync();
    }

    private static TransactionOutputEntity MapTransactionOutput(string TransactionId, ulong slot, TransactionOutput output)
    {
        return new TransactionOutputEntity
        {
            Id = TransactionId,
            Address = output.Address.ToBech32(),
            Slot = slot,
            Index = Convert.ToUInt32(output.Index),
            Amount = new ValueEntity
            {
                Coin = output.Amount.Coin,
                MultiAsset = output.Amount.MultiAsset.ToDictionary(
                    k => k.Key.ToHex(),
                    v => v.Value.ToDictionary(
                        k => k.Key.ToHex(),
                        v => v.Value
                    )
                )
            }
        };
    }
}