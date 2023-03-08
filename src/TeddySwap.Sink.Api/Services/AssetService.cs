using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Models;
using TeddySwap.Common.Models.CardanoDbSync;
using TeddySwap.Common.Enums;
using TeddySwap.Common.Models.Request;
using TeddySwap.Common.Models.Response;
using TeddySwap.Sink.Api.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Common.Services;

namespace TeddySwap.Sink.Api.Services;

public class AssetService
{
    private readonly ILogger<AssetService> _logger;
    private readonly CardanoDbSyncContext _dbContext;
    private readonly TeddySwapITNRewardSettings _settings;
    private readonly ByteArrayService _byteArrayService;

    public AssetService(
        ILogger<AssetService> logger,
        CardanoDbSyncContext dbContext,
        IOptions<TeddySwapITNRewardSettings> settings,
        ByteArrayService byteArrayService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _settings = settings.Value;
        _byteArrayService = byteArrayService;
    }

    public async Task<PaginatedAssetResponse> GetAssetsAsync(string policyId, string bech32Address, int offset, int limit, bool includeMetadata)
    {
        var unspentTxOuts = await _dbContext.TxOuts
            .Where(o => o.Address == bech32Address && !_dbContext.TxIns.Any(i => i.TxOutId == o.TxId && i.TxOutIndex == o.Index))
            .OrderBy(o => o.Id)
            .Select(o => o.Id)
            .ToListAsync();

        var policyBytes = _byteArrayService.HexToByteArray(policyId);

        var assets = await _dbContext.MaTxOuts
            .Where(maTxOut => maTxOut.IdentNavigation.Policy.SequenceEqual(policyBytes) && unspentTxOuts.Contains(maTxOut.TxOutId))
            .Select(maTxOut => new AssetResponse
            {
                Name = Encoding.UTF8.GetString(maTxOut.IdentNavigation.Name),
                Amount = (ulong)maTxOut.Quantity,
                MetadataJson =
                    includeMetadata &&
                    maTxOut != null &&
                    maTxOut.TxOut != null &&
                    maTxOut.TxOut.Tx != null &&
                    maTxOut.TxOut.Tx.TxMetadata.FirstOrDefault() != null ?
                    maTxOut.TxOut.Tx.TxMetadata.FirstOrDefault()!.Json : null
            })
            .ToListAsync();

        var totalCount = assets.Count;
        assets = assets.Skip(offset).Take(limit).ToList();

        return new PaginatedAssetResponse()
        {
            PolicyId = policyId,
            Address = bech32Address,
            TotalCount = totalCount,
            Result = assets
        };
    }
}