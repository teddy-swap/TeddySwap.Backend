using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.TxOutput)]
public class AssetReducer : OuraReducerBase
{
    private readonly IDbContextFactory<TeddySwapSinkCoreDbContext> _dbContextFactory;
    public AssetReducer(IDbContextFactory<TeddySwapSinkCoreDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task ReduceAsync(OuraAssetEvent asset)
    {
        if (asset.TxHash is not null &&
            asset.OutputIndex is not null &&
            asset.Context is not null &&
            asset.Context.BlockHash is not null)
        {
            using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

            await _dbContext.Assets.AddAsync(new Asset
            {
                PolicyId = asset.PolicyId ?? string.Empty,
                Name = asset.TokenName ?? string.Empty,
                Amount = asset.Amount,
                TxOutputHash = asset.TxHash,
                TxOutputIndex = (ulong)asset.OutputIndex,
                BlockHash = asset.Context.BlockHash
            });

            await _dbContext.SaveChangesAsync();
        }
    }
    public async Task RollbackAsync(Block rollbackBlock)
    {
        using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var assets = await _dbContext.Assets
            .Where(a => a.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.Assets.RemoveRange(assets);
        await _dbContext.SaveChangesAsync();
    }
}