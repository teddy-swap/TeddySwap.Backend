using System.Text.Json;
using CardanoSharp.Koios.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Filters;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Reducers;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Controllers;

[ApiController]
[Route("[controller]")]
public class OuraWebhookController : ControllerBase
{
    private readonly ILogger<OuraWebhookController> _logger;
    private readonly IDbContextFactory<TeddySwapSinkCoreDbContext> _coreDbContextFactory;
    private readonly IDbContextFactory<TeddySwapFisoSinkDbContext> _fisoDbContextFactory;
    private readonly IDbContextFactory<TeddySwapNftSinkDbContext> _nftDbContextFactory;
    private readonly IDbContextFactory<TeddySwapOrderSinkDbContext> _orderDbContextFactory;
    private readonly IDbContextFactory<TeddySwapBadgerAddressSinkDbContext> _badgerAddressDbContextFactory;
    private readonly JsonSerializerOptions ConclaveJsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true
    };
    private readonly CardanoService _cardanoService;
    private readonly IEnumerable<IOuraReducer> _reducers;
    private readonly IOptions<TeddySwapSinkSettings> _settings;
    private readonly CardanoFilterService _cardanoFilterService;

    public OuraWebhookController(
        ILogger<OuraWebhookController> logger,
        IDbContextFactory<TeddySwapSinkCoreDbContext> coreDbContextFactory,
        IDbContextFactory<TeddySwapFisoSinkDbContext> fisoDbContextFactory,
        IDbContextFactory<TeddySwapNftSinkDbContext> nftDbContextFactory,
        IDbContextFactory<TeddySwapOrderSinkDbContext> orderDbContextFactory,
        IDbContextFactory<TeddySwapBadgerAddressSinkDbContext> badgerAddressDbContextFactory,
        CardanoService cardanoService,
        IEnumerable<IOuraReducer> reducers,
        IOptions<TeddySwapSinkSettings> settings,
        CardanoFilterService cardanoFilterService
    )
    {
        _logger = logger;
        _coreDbContextFactory = coreDbContextFactory;
        _fisoDbContextFactory = fisoDbContextFactory;
        _nftDbContextFactory = nftDbContextFactory;
        _orderDbContextFactory = orderDbContextFactory;
        _badgerAddressDbContextFactory = badgerAddressDbContextFactory;
        _cardanoService = cardanoService;
        _reducers = reducers;
        _settings = settings;
        _cardanoFilterService = cardanoFilterService;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveEventAsync([FromBody] JsonElement _eventJson)
    {
        OuraEvent? _event = _eventJson.Deserialize<OuraEvent>(ConclaveJsonSerializerOptions);

        if (_event is not null && _event.Context is not null)
        {
            using var blockDbContext = await _CreateDbContextAsync(DbContextVariant.Core);
            if (_event.Variant == OuraVariant.RollBack)
            {
                OuraRollbackEvent? rollbackEvent = _eventJson.Deserialize<OuraRollbackEvent?>();
                if (rollbackEvent is not null && rollbackEvent.RollBack is not null && rollbackEvent.RollBack.BlockSlot is not null)
                {
                    _logger.LogInformation($"Rollback : Block Slot: {rollbackEvent.RollBack.BlockSlot}, Block Hash: {rollbackEvent.RollBack.BlockHash}");

                    BlockReducer? blockReducer = _reducers.Where(r => r is BlockReducer).FirstOrDefault() as BlockReducer;

                    if (blockReducer is not null)
                    {
                        await blockReducer.RollbackBySlotAsync((ulong)rollbackEvent.RollBack.BlockSlot, (blockDbContext as TeddySwapSinkCoreDbContext)!);
                    }
                }
            }
            else
            {
                _logger.LogInformation($"Event Received: {_event.Variant}, Block No: {_event.Context.BlockNumber}, Slot No: {_event.Context.Slot}, Block Hash: {_event.Context.BlockHash}");

                if (_event.Variant == OuraVariant.StakeDelegation)
                {
                    foreach (var reducer in _reducers)
                    {
                        List<OuraVariant> reducerVariants = _GetReducerVariants(reducer).ToList();

                        if (_settings.Value.Reducers.Any(rS => reducer.GetType().FullName?.Contains(rS) ?? false) || reducer is IOuraCoreReducer)
                        {
                            using var reducerDbContext = await _CreateDbContextAsync(_GetDbContextVariant(reducer));
                            foreach (var reducerVariant in reducerVariants)
                            {
                                switch (reducerVariant)
                                {
                                    case OuraVariant.StakeDelegation:
                                        await reducer.HandleReduceAsync(_eventJson.Deserialize<OuraStakeDelegationEvent>(ConclaveJsonSerializerOptions), reducerDbContext);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    OuraBlockEvent? blockEvent = null;
                    BlockReducer? blockReducer = _reducers.Where(r => r is BlockReducer).FirstOrDefault() as BlockReducer;
                    if (_event.Variant == OuraVariant.Block && blockReducer is not null)
                    {
                        blockEvent = _eventJson.Deserialize<OuraBlockEvent>(ConclaveJsonSerializerOptions);
                        if (blockEvent is not null && blockEvent.Block is not null)
                        {
                            if (blockEvent.Context is not null) blockEvent.Context.InvalidTransactions = blockEvent.Block.InvalidTransactions;
                            blockEvent = HydrateBlock(blockEvent);
                            await blockReducer.HandleReduceAsync(blockEvent, blockDbContext);
                        }
                    }

                    if (blockEvent is null || blockEvent.Block is null || blockEvent.Block.Transactions is null) return Ok();

                    List<OuraTransaction> transactions = _cardanoFilterService.FilterTransactions(blockEvent.Block.Transactions);
                    List<OuraTxInput> inputs = blockEvent.Block.Transactions.SelectMany(t => t.Inputs!).ToList();
                    List<OuraTxOutput> outputs = _cardanoFilterService.FilterTxOutputs(blockEvent.Block.Transactions.SelectMany(t => t.Outputs!).ToList());
                    List<OuraAssetEvent> assets = _cardanoFilterService.FilterAssets(MapToOuraAssetEvents(blockEvent.Block.Transactions.SelectMany(t => t.Outputs!)).ToList());
                    List<OuraTxInput> collateralInputs = blockEvent.Block.Transactions.Where(t => t.CollateralInputs is not null).SelectMany(t => t.CollateralInputs!).ToList();
                    List<OuraCollateralOutput> collateralOutputs = blockEvent.Block.Transactions.Where(t => t.CollateralOutput is not null).Select(t => t.CollateralOutput!).ToList();

                    IEnumerable<IOuraReducer> coreReducers = _reducers.Where(reducer => _settings.Value.Reducers.Any(rS => reducer.GetType().FullName?.Contains(rS) ?? false) && reducer is IOuraCoreReducer);
                    IEnumerable<IOuraReducer> otherReducers = _reducers.Where(reducer => _settings.Value.Reducers.Any(rS => reducer.GetType().FullName?.Contains(rS) ?? false) && reducer is not IOuraCoreReducer);

                    await HandleReducers(coreReducers, blockEvent, transactions, inputs, outputs, assets, collateralInputs, collateralOutputs);
                    await HandleReducers(otherReducers, blockEvent, transactions, inputs, outputs, assets, collateralInputs, collateralOutputs);

                    return Ok();
                }
            }
        }
        return Ok();
    }

    private ICollection<OuraVariant> _GetReducerVariants(IOuraReducer reducer)
    {
        OuraReducerAttribute? reducerAttribute = reducer.GetType().GetCustomAttributes(typeof(OuraReducerAttribute), true)
            .Where(
                reducerAttributeObject => reducerAttributeObject as OuraReducerAttribute is not null
            )
            .Select(reducerAttributeObject => reducerAttributeObject as OuraReducerAttribute).FirstOrDefault();
        return reducerAttribute?.Variants ?? new OuraVariant[] { OuraVariant.Unknown }.ToList();
    }

    private DbContextVariant _GetDbContextVariant(IOuraReducer reducer)
    {
        DbContextAttribute? dbContextAttribute = reducer.GetType().GetCustomAttributes(typeof(DbContextAttribute), true)
            .Where(dbContextAttribute => dbContextAttribute as DbContextAttribute is not null)
            .Select(dbContextAttributeObject => dbContextAttributeObject as DbContextAttribute)
            .FirstOrDefault();
        return dbContextAttribute?.Variants.FirstOrDefault() ?? DbContextVariant.Unknown;
    }

    private static List<OuraAssetEvent> MapToOuraAssetEvents(IEnumerable<OuraTxOutput>? outputs)
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

    private async Task<DbContext> _CreateDbContextAsync(DbContextVariant variant)
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

    private static OuraBlockEvent? HydrateBlock(OuraBlockEvent blockEvent)
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

    private async Task HandleReducers(
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
                ICollection<OuraVariant> reducerVariants = _GetReducerVariants(reducer);
                DbContextVariant reducerDbContextVariant = _GetDbContextVariant(reducer);
                using DbContext reducerDbContext = await _CreateDbContextAsync(reducerDbContextVariant);

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
}


