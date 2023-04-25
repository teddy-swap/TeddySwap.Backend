
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.Transaction)]
public class TestnetBadgerAddressReducer : OuraReducerBase
{
    private readonly IDbContextFactory<TeddySwapBadgerAddressSinkDbContext> _dbContextFactory;
    private readonly CardanoService _cardanoService;

    public TestnetBadgerAddressReducer(
        IDbContextFactory<TeddySwapBadgerAddressSinkDbContext> dbContextFactory,
        CardanoService cardanoService)
    {
        _dbContextFactory = dbContextFactory;
        _cardanoService = cardanoService;
    }

    public async Task ReduceAsync(OuraTransaction transaction)
    {
        if (transaction is not null &&
            transaction.Hash is not null &&
            transaction.Metadata is not null &&
            transaction.Context is not null &&
            transaction.Context.BlockHash is not null &&
            transaction.Context.Slot is not null)
        {
            if (transaction.Context.InvalidTransactions is not null && transaction.Context.InvalidTransactions.ToList().Contains((ulong)transaction.Index)) return;

            using TeddySwapBadgerAddressSinkDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

            if (_cardanoService.IsInvalidTransaction(transaction.Context.InvalidTransactions, (ulong)transaction.Index)) return;

            foreach (Metadatum metadata in transaction.Metadata)
            {
                if (metadata.Label != "848366") continue;

                string? address = transaction?.Outputs?.FirstOrDefault()?.Address;

                if (address is not null)
                {
                    string? stakeAddress = _cardanoService.TryGetStakeAddress(address);
                    string json = JsonSerializer.Serialize(metadata.Content);
                    Dictionary<string, List<string>>? testnetBadgerAddress = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);

                    if (testnetBadgerAddress is null) continue;

                    string? linkAddress = string.Join("", testnetBadgerAddress["mainnetAddress"]);
                    await _dbContext.BadgerAddressVerifications.AddAsync(new()
                    {
                        Address = address,
                        StakeAddress = stakeAddress,
                        LinkAddress = linkAddress,
                        TxHash = transaction.Hash,
                        Slot = (ulong)transaction.Context.Slot,
                        BlockHash = transaction.Context.BlockHash
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RollbackAsync(Block rollbackBlock)
    {
        using TeddySwapBadgerAddressSinkDbContext? _dbContext = await _dbContextFactory.CreateDbContextAsync();

        List<BadgerAddressVerification> badgerAddressVerifications = await _dbContext.BadgerAddressVerifications
            .Where(ba => ba.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.BadgerAddressVerifications.RemoveRange(badgerAddressVerifications);
        await _dbContext.SaveChangesAsync();
    }
}