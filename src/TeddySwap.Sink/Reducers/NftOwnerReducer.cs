using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.TxOutput)]
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

    public async Task ReduceAsync(OuraTxOutput txOutput)
    {

        if (txOutput is not null &&
            txOutput.Assets is not null &&
            txOutput.Address is not null)
        {
            using TeddySwapNftSinkDbContext? _dbContext = await _dbContextFactory.CreateDbContextAsync();

            foreach (OuraAsset asset in txOutput.Assets.Where(a => _settings.NftPolicyIds.Contains(a.Policy)))
            {
                if (asset.Policy is not null && asset.Asset is not null)
                {
                    NftOwner? owner = await _dbContext.NftOwners
                        .Where(n => n.PolicyId == asset.Policy.ToLower() && n.TokenName == asset.Asset.ToLower())
                        .FirstOrDefaultAsync();

                    if (owner is null)
                    {
                        await _dbContext.NftOwners.AddAsync(new()
                        {
                            Address = txOutput.Address,
                            PolicyId = asset.Policy.ToLower(),
                            TokenName = asset.Asset.ToLower(),
                        });
                    }
                    else
                    {
                        if (owner.Address != txOutput.Address)
                        {
                            owner.Address = txOutput.Address;
                            _dbContext.NftOwners.Update(owner);
                        }
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RollbackAsync(Block rollbackBlock)
    {
        if (rollbackBlock is not null)
        {
            using TeddySwapNftSinkDbContext? _dbContext = await _dbContextFactory.CreateDbContextAsync();
            List<Transaction>? transactions = await _dbContext.Transactions
                 .Include(tx => tx.Inputs)
                 .ThenInclude(i => i.TxOutput)
                 .ThenInclude(o => o.Assets)
                 .Where(t => t.Block == rollbackBlock)
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

                    // delete other asset inputs included in the transaction
                    foreach (TxInput txInput in transaction.Inputs)
                    {
                        if (txInput.TxOutput.Assets is null) continue;
                        foreach (Asset asset in txInput.TxOutput.Assets)
                        {
                            if (_settings.NftPolicyIds.Contains(asset.PolicyId.ToLower()))
                            {

                                NftOwner? owner = await _dbContext.NftOwners
                                    .Where(n => n.PolicyId.ToLower() == asset.PolicyId.ToLower() &&
                                        n.TokenName.ToLower() == asset.Name.ToLower())
                                    .FirstOrDefaultAsync();

                                if (owner is null) continue;

                                if (owner.Address != txInput.TxOutput.Address)
                                {
                                    owner.Address = txInput.TxOutput.Address;
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
}