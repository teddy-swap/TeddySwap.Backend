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
using System.Numerics;
using TeddySwap.Common.Models.Explorer;

namespace TeddySwap.Sink.Api.Services;

public class OutputService
{
    private readonly ILogger<OutputService> _logger;
    private readonly CardanoDbSyncContext _dbContext;
    private readonly TeddySwapITNRewardSettings _settings;
    private readonly ByteArrayService _byteArrayService;

    public OutputService(
        ILogger<OutputService> logger,
        CardanoDbSyncContext dbContext,
        IOptions<TeddySwapITNRewardSettings> settings,
        ByteArrayService byteArrayService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _settings = settings.Value;
        _byteArrayService = byteArrayService;
    }


    public async Task<PaginatedOutputResponse> GetUtxosByPaymentKeyHashAsync(int offset, int limit, string pkh)
    {
        var bytePkh = _byteArrayService.HexToByteArray(pkh);
        List<OutputResponse> utxos = await _dbContext.TxOuts
            .Include(o => o.Tx)
            .ThenInclude(tx => tx.Block)
            .Include(o => o.MaTxOuts)
            .ThenInclude(ma => ma.IdentNavigation)
            .Where(o => o.PaymentCred != null && o.PaymentCred.SequenceEqual(bytePkh) && !_dbContext.TxIns.Any(i => i.TxOutId == o.TxId && i.TxOutIndex == o.Index))
            .Skip(offset)
            .Take(limit)
            .Select(o => new OutputResponse
            {
                Output = new()
                {

                    BlockHash = Convert.ToHexString(o.Tx.Block.Hash).ToLower(),
                    TxHash = Convert.ToHexString(o.Tx.Hash).ToLower(),
                    Index = o.Index,
                    GlobalIndex = o.Id,
                    Address = o.Address,
                    RawAddr = Convert.ToHexString(o.AddressRaw).ToLower(),
                    PaymentCred = o.PaymentCred != null ? Convert.ToHexString(o.PaymentCred).ToLower() : null,
                    Lovelace = o.Value.ToString(),
                    Value = o.MaTxOuts
                        .Select(ma => new OutputAsset()
                        {
                            PolicyId = Convert.ToHexString(ma.IdentNavigation.Policy).ToLower(),
                            Name = Encoding.UTF8.GetString(ma.IdentNavigation.Name),
                            Quantity = ma.Quantity.ToString(),
                        })
                        .ToList(),
                    DataHash = o.DataHash != null ? Convert.ToHexString(o.DataHash).ToLower() : null,
                    Data = o.InlineDatum,
                    DataBin = null,
                    RefScriptHash = o.ReferenceScript != null ? Convert.ToHexString(o.ReferenceScript.Hash).ToLower() : null
                }
            })
            .ToListAsync();

        var totalUtxos = await CountUtxosByPaymentKeyHashAsync(pkh);

        return new()
        {
            Result = utxos,
            TotalCount = totalUtxos
        };
    }

    public async Task<List<OutputResponse>> GetUtxosByAddressAsync(int offset, int limit, string address)
    {
        List<OutputResponse> utxos = await _dbContext.TxOuts
            .Include(o => o.Tx)
            .ThenInclude(tx => tx.Block)
            .Include(o => o.MaTxOuts)
            .ThenInclude(ma => ma.IdentNavigation)
            .Where(o => o.Address == address && !_dbContext.TxIns.Any(i => i.TxOutId == o.TxId && i.TxOutIndex == o.Index))
            .Skip(offset)
            .Take(limit)
            .Select(o => new OutputResponse
            {
                Output = new()
                {

                    BlockHash = Convert.ToHexString(o.Tx.Block.Hash).ToLower(),
                    TxHash = Convert.ToHexString(o.Tx.Hash).ToLower(),
                    Index = o.Index,
                    GlobalIndex = o.Id,
                    Address = o.Address,
                    RawAddr = Convert.ToHexString(o.AddressRaw).ToLower(),
                    PaymentCred = o.PaymentCred != null ? Convert.ToHexString(o.PaymentCred).ToLower() : null,
                    Lovelace = o.Value.ToString(),
                    Value = o.MaTxOuts
                        .Select(ma => new OutputAsset()
                        {
                            PolicyId = Convert.ToHexString(ma.IdentNavigation.Policy).ToLower(),
                            Name = Encoding.UTF8.GetString(ma.IdentNavigation.Name),
                            Quantity = ma.Quantity.ToString(),
                        })
                        .ToList(),
                    DataHash = o.DataHash != null ? Convert.ToHexString(o.DataHash).ToLower() : null,
                    Data = o.InlineDatum,
                    DataBin = null,
                    RefScriptHash = o.ReferenceScript != null ? Convert.ToHexString(o.ReferenceScript.Hash).ToLower() : null
                }
            })
            .ToListAsync();

        return utxos;
    }

    public async Task<PaginatedOutputResponse> GetUtxosByPaymentKeyHashWithAssets(string pkh, List<AssetClass> containsAnyOf, int offset, int limit)
    {
        var bytePkh = _byteArrayService.HexToByteArray(pkh);
        var query = _dbContext.TxOuts
            .Include(t => t.Tx)
            .ThenInclude(t => t.Block)
            .Include(t => t.MaTxOuts)
            .ThenInclude(ma => ma.IdentNavigation)
            .Include(t => t.Tx)
            .Include(t => t.ReferenceScript)
            .Where(o => o.PaymentCred != null && o.PaymentCred.SequenceEqual(bytePkh) && !_dbContext.TxIns.Any(i => i.TxOutId == o.TxId && i.TxOutIndex == o.Index));

        if (containsAnyOf is not null && containsAnyOf.Count > 0)
        {
            query = query
                .Where(t => containsAnyOf.Any(a =>
                    t.MaTxOuts.Any(ma => ma.IdentNavigation.Policy == _byteArrayService.HexToByteArray(a.PolicyId) &&
                        Encoding.UTF8.GetString(ma.IdentNavigation.Name) == a.Name)));
        }

        var outputs = query
            .OrderByDescending(t => t.TxId)
            .ThenByDescending(t => t.Index)
            .Skip(offset)
            .Take(limit);

        List<OutputResponse> result = await outputs.Select(o => new OutputResponse
        {
            Output = new Output
            {
                BlockHash = Convert.ToHexString(o.Tx.Block.Hash).ToLower(),
                TxHash = Convert.ToHexString(o.Tx.Hash).ToLower(),
                Index = o.Index,
                GlobalIndex = o.Id,
                Address = o.Address,
                RawAddr = Convert.ToHexString(o.AddressRaw).ToLower(),
                PaymentCred = o.PaymentCred != null ? Convert.ToHexString(o.PaymentCred).ToLower() : null,
                Lovelace = o.Value.ToString(),
                Value = o.MaTxOuts
                            .Select(ma => new OutputAsset()
                            {
                                PolicyId = Convert.ToHexString(ma.IdentNavigation.Policy).ToLower(),
                                Name = Encoding.UTF8.GetString(ma.IdentNavigation.Name),
                                Quantity = ma.Quantity.ToString(),
                            })
                            .ToList(),
                DataHash = o.DataHash != null ? Convert.ToHexString(o.DataHash).ToLower() : null,
                Data = o.InlineDatum,
                DataBin = null,
                RefScriptHash = o.ReferenceScript != null ? Convert.ToHexString(o.ReferenceScript.Hash).ToLower() : null
            }
        }).ToListAsync();


        return new()
        {
            Result = result,
            TotalCount = 0
        };
    }

    public async Task<int> CountUtxosByPaymentKeyHashAsync(string pkh)
    {
        var bytePkh = _byteArrayService.HexToByteArray(pkh);
        int count = await _dbContext.TxOuts
            .Where(o => o.PaymentCred != null && o.PaymentCred.SequenceEqual(bytePkh) && !_dbContext.TxIns.Any(i => i.TxOutId == o.TxId && i.TxOutIndex == o.Index))
            .CountAsync();

        return count;
    }

}