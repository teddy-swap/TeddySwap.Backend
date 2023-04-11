using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.Asset)]
public class NftOwnerReducer : OuraReducerBase
{
    private readonly ILogger<NftOwnerReducer> _logger;
    private readonly IDbContextFactory<TeddySwapNftSinkDbContext> _dbContextFactory;
    private readonly TeddySwapSinkSettings _settings;
    private readonly MetadataService _metadataService;

    public NftOwnerReducer(
        ILogger<NftOwnerReducer> logger,
        IDbContextFactory<TeddySwapNftSinkDbContext> dbContextFactory,
        IOptions<TeddySwapSinkSettings> settings,
        MetadataService metadataService)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _settings = settings.Value;
        _metadataService = metadataService;
    }

    public async Task ReduceAsync(OuraAssetEvent asset)
    {
        if (asset is not null &&
            asset.Address is not null &&
            !string.IsNullOrEmpty(asset.PolicyId) &&
            !string.IsNullOrEmpty(asset.TokenName))
        {
            // skip invalid transactions
            if (asset.Context is not null &&
                asset.Context.InvalidTransactions is not null &&
                asset.Context.InvalidTransactions.ToList().Contains((ulong)asset.Context.TxIdx!)) return;

            if (_settings.NftPolicyIds.Contains(asset.PolicyId))
            {
                using TeddySwapNftSinkDbContext? _dbContext = await _dbContextFactory.CreateDbContextAsync();

                NftOwner? owner = await _dbContext.NftOwners
                    .Where(n => n.PolicyId == asset.PolicyId.ToLower() && n.TokenName == asset.TokenName.ToLower())
                    .FirstOrDefaultAsync();

                if (owner is null)
                {
                    await _dbContext.NftOwners.AddAsync(new()
                    {
                        Address = asset.Address,
                        PolicyId = asset.PolicyId.ToLower(),
                        TokenName = asset.TokenName.ToLower(),
                    });
                }
                else
                {
                    if (owner.Address != asset.Address)
                    {
                        owner.Address = asset.Address;
                        _dbContext.NftOwners.Update(owner);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    public async Task RollbackAsync(Block rollbackBlock)
    {

        using TeddySwapNftSinkDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

        List<Transaction>? transactions = await _dbContext.Transactions
             .Where(t => t.Blockhash == rollbackBlock.BlockHash)
             .ToListAsync();

        if (transactions is not null)
        {
            foreach (Transaction transaction in transactions)
            {
                // Check for rollback mint transactions and delete nft owners
                if (transaction.Metadata is not null)
                {
                    List<AssetClass> assetClasses = _metadataService.FindAssets(transaction, _settings.NftPolicyIds.ToList());

                    foreach (AssetClass asset in assetClasses)
                    {
                        NftOwner? owner = await _dbContext.NftOwners
                            .Where(n => n.PolicyId.ToLower() == asset.PolicyId.ToLower() &&
                                n.TokenName.ToLower() == asset.Name.ToLower())
                            .FirstOrDefaultAsync();

                        if (owner is null) continue;

                        // If mint transaction rollback, delete entry
                        if (assetClasses.Any(a => a.PolicyId == owner.PolicyId && a.Name == owner.TokenName))
                        {
                            _dbContext.NftOwners.Remove(owner);
                        }
                    }
                }

                var inputs = await _dbContext.TxInputs
                    .Where(i => i.TxHash == transaction.Hash)
                    .ToListAsync();

                // delete other asset inputs included in the transaction
                foreach (TxInput txInput in inputs)
                {
                    var output = await _dbContext.TxOutputs
                        .Where(o => o.TxHash == txInput.TxOutputHash && o.Index == txInput.TxOutputIndex)
                        .FirstOrDefaultAsync();

                    if (output is null) continue;

                    var assets = await _dbContext.Assets
                        .Where(a => a.TxOutputHash == output.TxHash && a.TxOutputIndex == output.Index)
                        .ToListAsync();

                    if (!assets.Any()) continue;

                    foreach (Asset asset in assets)
                    {
                        if (_settings.NftPolicyIds.Contains(asset.PolicyId.ToLower()))
                        {

                            NftOwner? owner = await _dbContext.NftOwners
                                .Where(n => n.PolicyId.ToLower() == asset.PolicyId.ToLower() &&
                                    n.TokenName.ToLower() == asset.Name.ToLower())
                                .FirstOrDefaultAsync();

                            if (owner is null) continue;

                            if (owner.Address != output.Address)
                            {
                                owner.Address = output.Address;
                                _dbContext.NftOwners.Update(owner);
                            }
                        }
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
        }

    }
}