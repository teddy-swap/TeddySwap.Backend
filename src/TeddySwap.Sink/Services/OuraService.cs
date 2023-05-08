using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Reducers;

namespace TeddySwap.Sink.Services;

public class OuraService
{
    private readonly IDbContextFactory<TeddySwapSinkCoreDbContext> _coreDbContextFactory;
    private readonly IDbContextFactory<TeddySwapFisoSinkDbContext> _fisoDbContextFactory;
    private readonly IDbContextFactory<TeddySwapNftSinkDbContext> _nftDbContextFactory;
    private readonly IDbContextFactory<TeddySwapOrderSinkDbContext> _orderDbContextFactory;
    private readonly IDbContextFactory<TeddySwapBadgerAddressSinkDbContext> _badgerAddressDbContextFactory;

    public OuraService(
        IDbContextFactory<TeddySwapSinkCoreDbContext> coreDbContextFactory,
        IDbContextFactory<TeddySwapFisoSinkDbContext> fisoDbContextFactory,
        IDbContextFactory<TeddySwapNftSinkDbContext> nftDbContextFactory,
        IDbContextFactory<TeddySwapOrderSinkDbContext> orderDbContextFactory,
        IDbContextFactory<TeddySwapBadgerAddressSinkDbContext> badgerAddressDbContextFactory)
    {
        _coreDbContextFactory = coreDbContextFactory;
        _fisoDbContextFactory = fisoDbContextFactory;
        _nftDbContextFactory = nftDbContextFactory;
        _orderDbContextFactory = orderDbContextFactory;
        _badgerAddressDbContextFactory = badgerAddressDbContextFactory;
    }

    public ICollection<OuraVariant> GetReducerVariants(IOuraReducer reducer)
    {
        OuraReducerAttribute? reducerAttribute = reducer.GetType().GetCustomAttributes(typeof(OuraReducerAttribute), true)
            .Where(
                reducerAttributeObject => reducerAttributeObject as OuraReducerAttribute is not null
            )
            .Select(reducerAttributeObject => reducerAttributeObject as OuraReducerAttribute).FirstOrDefault();
        return reducerAttribute?.Variants ?? new OuraVariant[] { OuraVariant.Unknown }.ToList();
    }

    public DbContextVariant GetDbContextVariant(IOuraReducer reducer)
    {
        DbContextAttribute? dbContextAttribute = reducer.GetType().GetCustomAttributes(typeof(DbContextAttribute), true)
            .Where(dbContextAttribute => dbContextAttribute as DbContextAttribute is not null)
            .Select(dbContextAttributeObject => dbContextAttributeObject as DbContextAttribute)
            .FirstOrDefault();
        return dbContextAttribute?.Variants.FirstOrDefault() ?? DbContextVariant.Unknown;
    }

    public List<OuraAssetEvent> MapToOuraAssetEvents(IEnumerable<OuraTxOutput>? outputs)
    {
        if (outputs is null) return new();

        var assets = outputs
            .Where(o => o.Assets is not null && o.Assets.Any())
            .SelectMany(o => o.Assets!.Select(a => new OuraAssetEvent()
            {
                Address = o.Address ?? "",
                PolicyId = a.Policy ?? "",
                TokenName = a.Asset ?? "",
                Amount = a.Amount is not null ? (ulong)a.Amount : 0,
                Context = o.Context,
                TxHash = o.TxHash,
                OutputIndex = o.OutputIndex
            }))
            .ToList();

        return assets;
    }

    public async Task<DbContext> CreateDbContextAsync(DbContextVariant variant)
    {
        return variant switch
        {
            DbContextVariant.Core => await _coreDbContextFactory.CreateDbContextAsync(),
            DbContextVariant.Fiso => await _fisoDbContextFactory.CreateDbContextAsync(),
            DbContextVariant.Nft => await _nftDbContextFactory.CreateDbContextAsync(),
            DbContextVariant.Order => await _orderDbContextFactory.CreateDbContextAsync(),
            DbContextVariant.BadgerAddress => await _badgerAddressDbContextFactory.CreateDbContextAsync(),
            _ => throw new Exception("Invalid DbContext Variant")
        };
    }

    public async Task HandleReducers(
        IEnumerable<IOuraReducer> reducers,
        OuraBlockEvent blockEvent,
        List<OuraTransaction> transactions,
        List<OuraTxInput> inputs,
        List<OuraTxOutput> outputs,
        List<OuraAssetEvent> assets,
        List<OuraTxInput> collateralInputs,
        List<OuraCollateralOutput> collateralOutputs)
    {
        var reducerTasks = reducers.Select(async reducer =>
            {
                ICollection<OuraVariant> reducerVariants = GetReducerVariants(reducer);
                DbContextVariant reducerDbContextVariant = GetDbContextVariant(reducer);
                using DbContext reducerDbContext = await CreateDbContextAsync(reducerDbContextVariant);

                foreach (var reducerVariant in reducerVariants)
                {
                    switch (reducerVariant)
                    {
                        case OuraVariant.Block:
                            await reducer.HandleReduceAsync(blockEvent, reducerDbContext);
                            break;
                        case OuraVariant.Transaction:
                            foreach (var transaction in transactions) await reducer.HandleReduceAsync(transaction, reducerDbContext);
                            break;
                        case OuraVariant.TxInput:
                            foreach (var input in inputs) await reducer.HandleReduceAsync(input, reducerDbContext);
                            break;
                        case OuraVariant.TxOutput:
                            foreach (var output in outputs) await reducer.HandleReduceAsync(output, reducerDbContext);
                            break;
                        case OuraVariant.Asset:
                            foreach (var asset in assets) await reducer.HandleReduceAsync(asset, reducerDbContext);
                            break;
                        case OuraVariant.CollateralInput:
                            foreach (var collateralInput in collateralInputs) await reducer.HandleReduceAsync(collateralInput, reducerDbContext);
                            break;
                        case OuraVariant.CollateralOutput:
                            foreach (var collateralOutput in collateralOutputs) await reducer.HandleReduceAsync(collateralOutput, reducerDbContext);
                            break;
                        default:
                            break;
                    }
                }
            });
        await Task.WhenAll(reducerTasks);
    }

    public OuraBlockEvent? HydrateBlock(OuraBlockEvent blockEvent)
    {
        if (blockEvent.Block is null) return null;
        blockEvent.Block.Transactions = blockEvent.Block.Transactions?.Select((t, ti) =>
         {
             t.Index = ti;
             t.Context = blockEvent.Context;
             t.Context!.TxHash = t.Hash;
             t.Outputs = t.Outputs?.Select((o, oi) =>
             {
                 o.Context = blockEvent.Context;
                 o.OutputIndex = (ulong)oi;
                 o.TxHash = t.Hash;
                 o.TxIndex = (ulong)ti;
                 o.Variant = OuraVariant.TxOutput;
                 return o;
             });
             t.Inputs = t.Inputs?.Select(i =>
             {
                 i.Context = blockEvent.Context;
                 i.Context!.TxIdx = (ulong)ti;
                 i.Variant = OuraVariant.TxInput;
                 return i;
             });
             t.CollateralInputs = t.CollateralInputs?.Select(ci =>
             {
                 ci.Context = blockEvent.Context;
                 ci.Context!.TxIdx = (ulong)ti;
                 ci.Variant = OuraVariant.CollateralInput;
                 return ci;
             });
             if (t.HasCollateralOutput)
             {
                 t.CollateralOutput!.Context = blockEvent.Context;
                 t.CollateralOutput.Context!.HasCollateralOutput = t.HasCollateralOutput;
                 t.CollateralOutput.Context.TxHash = t.Hash;
                 t.CollateralOutput.Variant = OuraVariant.CollateralOutput;
             }
             return t;
         }).ToList();

        return blockEvent;
    }
}