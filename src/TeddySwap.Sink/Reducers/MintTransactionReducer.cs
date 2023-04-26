
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.Transaction)]
[DbContext(DbContextVariant.Nft)]
public class MintTransactionReducer : OuraReducerBase
{
    private readonly TeddySwapSinkSettings _settings;
    private readonly MetadataService _metadataService;
    private readonly CardanoService _cardanoService;

    public MintTransactionReducer(
        IOptions<TeddySwapSinkSettings> settings,
        MetadataService metadataService,
        CardanoService cardanoService)
    {
        _settings = settings.Value;
        _metadataService = metadataService;
        _cardanoService = cardanoService;
    }

    public async Task ReduceAsync(OuraTransaction transaction, TeddySwapNftSinkDbContext _dbContext)
    {
        if (transaction is not null &&
            transaction.Hash is not null &&
            transaction.Fee is not null &&
            transaction.Metadata is not null &&
            transaction.Mint is not null &&
            transaction.Mint.Any() &&
            transaction.Context is not null &&
            transaction.Context.BlockHash is not null &&
            transaction.Context.Slot is not null)
        {
            if (transaction.Context.InvalidTransactions is not null && transaction.Context.InvalidTransactions.ToList().Contains((ulong)transaction.Index)) return;
            if (_cardanoService.IsInvalidTransaction(transaction.Context.InvalidTransactions, (ulong)transaction.Index)) return;

            List<AssetClass> assetWithMetada = _metadataService.FindAssets(transaction, _settings.NftPolicyIds.ToList());

            foreach (MintAsset asset in transaction.Mint)
            {
                if (string.IsNullOrEmpty(asset.Policy) ||
                    string.IsNullOrEmpty(asset.Asset) ||
                    !_settings.NftPolicyIds.ToList().Contains(asset.Policy)) continue;

                string? metadata = assetWithMetada
                    .Where(awm => awm.PolicyId == asset.Policy && (awm.Name == asset.Asset || awm.AsciiName == asset.Asset))
                    .Select(a => a.Metadata)
                    .FirstOrDefault();

                await _dbContext.MintTransactions.AddAsync(new()
                {
                    PolicyId = asset.Policy.ToLower(),
                    TokenName = asset.Asset.ToLower(),
                    AsciiTokenName = Encoding.ASCII.GetString(Convert.FromHexString(asset.Asset).Where(b => b < 128 && b != 0x00).ToArray()),
                    Metadata = metadata,
                    TxHash = transaction.Hash,
                    BlockHash = transaction.Context.BlockHash,
                    Slot = (ulong)transaction.Context.Slot
                });
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RollbackAsync(Block rollbackBlock, TeddySwapNftSinkDbContext _dbContext)
    {
        List<MintTransaction> mintTransactions = await _dbContext.MintTransactions
            .Where(mtx => mtx.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.MintTransactions.RemoveRange(mintTransactions);
        await _dbContext.SaveChangesAsync();
    }
}