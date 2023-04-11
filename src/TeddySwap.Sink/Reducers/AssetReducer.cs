using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.TxOutput)]
public class AssetReducer : OuraReducerBase
{
    private readonly ILogger<AssetReducer> _logger;
    private IDbContextFactory<TeddySwapSinkCoreDbContext> _dbContextFactory;
    public AssetReducer(
        ILogger<AssetReducer> logger,
        IDbContextFactory<TeddySwapSinkCoreDbContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task ReduceAsync(OuraAssetEvent asset)
    {
        if (asset.TxHash is not null && asset.OutputIndex is not null)
        {

            using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

            TxOutput? existingOutput = await _dbContext.TxOutputs
                .Where(o => o.TxHash == asset.TxHash && o.Index == asset.OutputIndex)
                .FirstOrDefaultAsync();

            if (existingOutput is null) return;

            await _dbContext.Assets.AddAsync(new Asset
            {
                PolicyId = asset.PolicyId ?? string.Empty,
                Name = asset.TokenName ?? string.Empty,
                Amount = asset.Amount,
                TxOutput = existingOutput
            });

            await _dbContext.SaveChangesAsync();
        }
    }
    public async Task RollbackAsync(Block _) => await Task.CompletedTask;
}