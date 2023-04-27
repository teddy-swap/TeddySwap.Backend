using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.Transaction)]
[DbContext(DbContextVariant.BadgerAddress)]
public class TestnetBadgerAddressReducer : OuraReducerBase
{
    private readonly CardanoService _cardanoService;

    public TestnetBadgerAddressReducer(CardanoService cardanoService)
    {
        _cardanoService = cardanoService;
    }

    public async Task ReduceAsync(OuraTransaction transaction, TeddySwapBadgerAddressSinkDbContext _dbContext)
    {
        if (transaction is not null &&
            transaction.Hash is not null &&
            transaction.Metadata is not null &&
            transaction.Context is not null &&
            transaction.Context.BlockHash is not null &&
            transaction.Context.Slot is not null)
        {
            if (transaction.Context.InvalidTransactions is not null && transaction.Context.InvalidTransactions.ToList().Contains((ulong)transaction.Index)) return;
            if (_cardanoService.IsInvalidTransaction(transaction.Context.InvalidTransactions, (ulong)transaction.Index)) return;

            foreach (Metadatum metadata in transaction.Metadata)
            {
                if (metadata.Label != "848366") continue;

                string? address = transaction?.Outputs?.FirstOrDefault()?.Address;

                if (address is not null)
                {
                    string json = JsonSerializer.Serialize(metadata.Content);
                    Dictionary<string, List<string>>? testnetBadgerAddress = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);

                    if (testnetBadgerAddress is null) continue;

                    string? linkAddress = string.Join("", testnetBadgerAddress["mainnetAddress"]);
                    string? stakeAddress = _cardanoService.TryGetStakeAddress(address);
                    string? linkStakeAddress = _cardanoService.TryGetStakeAddress(linkAddress);

                    await _dbContext.BadgerAddressVerifications.AddAsync(new()
                    {
                        Address = address,
                        StakeAddress = stakeAddress,
                        LinkAddress = linkAddress,
                        LinkStakeAddress = linkStakeAddress,
                        TxHash = transaction.Hash,
                        Slot = (ulong)transaction.Context.Slot,
                        BlockHash = transaction.Context.BlockHash
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RollbackAsync(Block rollbackBlock, TeddySwapBadgerAddressSinkDbContext _dbContext)
    {
        List<BadgerAddressVerification> badgerAddressVerifications = await _dbContext.BadgerAddressVerifications
            .Where(ba => ba.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.BadgerAddressVerifications.RemoveRange(badgerAddressVerifications);
        await _dbContext.SaveChangesAsync();
    }
}