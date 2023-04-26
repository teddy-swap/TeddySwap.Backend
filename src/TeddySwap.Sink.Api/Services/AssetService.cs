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

namespace TeddySwap.Sink.Api.Services;

public class AssetService
{
    private readonly ILogger<AssetService> _logger;
    private readonly TeddySwapNftSinkDbContext _dbContext;
    private readonly TeddySwapITNRewardSettings _settings;

    public AssetService(
        ILogger<AssetService> logger,
        TeddySwapNftSinkDbContext dbContext,
        IOptions<TeddySwapITNRewardSettings> settings)
    {
        _logger = logger;
        _dbContext = dbContext;
        _settings = settings.Value;
    }

    public async Task<PaginatedAssetResponse> GetNftOwnerAsync(int offset, int limit, string address, string policyId)
    {
        var nftOwnerQuery = _dbContext.NftOwners
            .Where(no => no.Address == address && no.PolicyId == policyId.ToLower());

        var totalNfts = await nftOwnerQuery.CountAsync();

        var mintTransactions = _dbContext.MintTransactions
            .Where(mtx => mtx.PolicyId == policyId)
            .Include(mtx => mtx.Transaction)
            .ThenInclude(tx => tx.Block)
            .OrderBy(mtx => mtx.Transaction.Block.Slot)
            .ThenBy(mtx => mtx.Transaction.Index)
            .Select(mtx => mtx.TokenName)
            .ToList()
            .Select(selector: (tn, i) => new
            {
                RowNumber = i + 1,
                TokenName = tn
            });

        List<AssetResponse> paginatedNfts = await nftOwnerQuery
            .Skip(offset)
            .Take(limit)
            .Select(no => new AssetResponse()
            {
                Name = no.TokenName,
                AsciiName = Encoding.ASCII.GetString(Convert.FromHexString(no.TokenName)),
                Amount = 1
            })
            .ToListAsync();

        var filteredMintTransactions = mintTransactions
            .Where(mtx => paginatedNfts.Select(pn => pn.Name).ToList().Contains(mtx.TokenName))
            .ToList();

        paginatedNfts.ForEach(pn => pn.MintOrder = filteredMintTransactions.FirstOrDefault(fmtx => fmtx.TokenName == pn.Name)!.RowNumber);

        return new()
        {
            TotalCount = totalNfts,
            PolicyId = policyId,
            Result = paginatedNfts,
            Address = address
        };
    }

    public async Task<PaginatedAssetResponse> GetNftOwnerByStakeAddressAsync(int offset, int limit, string stakeAddress, string policyId)
    {
        var nftOwnerQuery = _dbContext.NftOwners
            .Where(no => !string.IsNullOrEmpty(no.StakeAddress))
            .Where(no => no.StakeAddress == stakeAddress && no.PolicyId == policyId.ToLower());

        var totalNfts = await nftOwnerQuery.CountAsync();

        var mintTransactions = _dbContext.MintTransactions
            .Where(mtx => mtx.PolicyId == policyId)
            .Include(mtx => mtx.Transaction)
            .ThenInclude(tx => tx.Block)
            .OrderBy(mtx => mtx.Transaction.Block.Slot)
            .ThenBy(mtx => mtx.Transaction.Index)
            .Select(mtx => mtx.TokenName)
            .ToList()
            .Select(selector: (tn, i) => new
            {
                RowNumber = i + 1,
                TokenName = tn
            });

        List<AssetResponse> paginatedNfts = await nftOwnerQuery
            .Skip(offset)
            .Take(limit)
            .Select(no => new AssetResponse()
            {
                Name = no.TokenName,
                AsciiName = Encoding.ASCII.GetString(Convert.FromHexString(no.TokenName)),
                Amount = 1
            })
            .ToListAsync();

        var filteredMintTransactions = mintTransactions
            .Where(mtx => paginatedNfts.Select(pn => pn.Name).ToList().Contains(mtx.TokenName))
            .ToList();

        paginatedNfts.ForEach(pn => pn.MintOrder = filteredMintTransactions.FirstOrDefault(fmtx => fmtx.TokenName == pn.Name)!.RowNumber);

        return new()
        {
            TotalCount = totalNfts,
            PolicyId = policyId,
            Result = paginatedNfts,
            Address = stakeAddress
        };
    }

    public async Task<PaginatedAssetResponse> GetNftOwnersAsync(int offset, int limit, List<string> addresses, string policyId)
    {
        var nftOwnerQuery = _dbContext.NftOwners
            .Where(no => addresses.Contains(no.Address) && no.PolicyId == policyId.ToLower());

        var totalNfts = await nftOwnerQuery.CountAsync();

        List<AssetResponse> paginatedNfts = await nftOwnerQuery
            .Skip(offset)
            .Take(limit)
            .Select(no => new AssetResponse()
            {
                Name = no.TokenName,
                AsciiName = Encoding.ASCII.GetString(Convert.FromHexString(no.TokenName)),
                Amount = 1
            })
            .ToListAsync();

        return new()
        {
            TotalCount = totalNfts,
            PolicyId = policyId,
            Result = paginatedNfts,
            Address = addresses.FirstOrDefault() ?? ""
        };
    }

    public async Task<AssetMetadataResponse?> GetNftMetadataAsync(string policyId, string tokenName)
    {
        return await _dbContext.MintTransactions
            .Where(mtx => mtx.PolicyId == policyId.ToLower() && mtx.TokenName == tokenName.ToLower())
            .Select(m => new AssetMetadataResponse()
            {
                PolicyId = m.PolicyId,
                TokenName = m.TokenName,
                AsciiTokenName = m.AsciiTokenName,
                Metadata = m.Metadata
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Dictionary<string, AssetMetadataResponse>?> GetNftMetadataAsync(List<string> assets)
    {
        var metadata = await _dbContext.MintTransactions
            .Where(mtx => assets.Contains(mtx.PolicyId + mtx.TokenName))
            .Select(m => new AssetMetadataResponse()
            {
                PolicyId = m.PolicyId,
                TokenName = m.TokenName,
                AsciiTokenName = m.AsciiTokenName,
                Metadata = m.Metadata
            })
            .ToListAsync();

        Dictionary<string, AssetMetadataResponse> metadataDictionary = new();
        metadata.ForEach(m => { metadataDictionary.Add(m.PolicyId + m.TokenName, m); });
        return metadataDictionary;
    }
}