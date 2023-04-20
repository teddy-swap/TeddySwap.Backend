using System.Text.Json;
using CardanoSharp.Koios.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Filters;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Reducers;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Controllers;

[ApiController]
[Route("[controller]")]
public class OuraWebhookController : ControllerBase
{
    private readonly ILogger<OuraWebhookController> _logger;
    private readonly IDbContextFactory<TeddySwapSinkCoreDbContext> _dbContextFactory;
    private readonly JsonSerializerOptions ConclaveJsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true
    };
    private readonly CardanoService _cardanoService;
    private readonly IEnumerable<IOuraReducer> _reducers;
    private readonly IOptions<TeddySwapSinkSettings> _settings;
    private readonly CardanoFilters _cardanoFilters;

    public OuraWebhookController(
        ILogger<OuraWebhookController> logger,
        IDbContextFactory<TeddySwapSinkCoreDbContext> dbContextFactory,
        CardanoService cardanoService,
        IEnumerable<IOuraReducer> reducers,
        IOptions<TeddySwapSinkSettings> settings,
        CardanoFilters cardanoFilters
    )
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _cardanoService = cardanoService;
        _reducers = reducers;
        _settings = settings;
        _cardanoFilters = cardanoFilters;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveEventAsync([FromBody] JsonElement _eventJson)
    {
        OuraEvent? _event = _eventJson.Deserialize<OuraEvent>(ConclaveJsonSerializerOptions);

        if (_event is not null && _event.Context is not null)
        {
            if (_event.Variant == OuraVariant.RollBack)
            {
                OuraRollbackEvent? rollbackEvent = _eventJson.Deserialize<OuraRollbackEvent?>();
                if (rollbackEvent is not null && rollbackEvent.RollBack is not null && rollbackEvent.RollBack.BlockSlot is not null)
                {
                    _logger.LogInformation($"Rollback : Block Slot: {rollbackEvent.RollBack.BlockSlot}, Block Hash: {rollbackEvent.RollBack.BlockHash}");

                    BlockReducer? blockReducer = _reducers.Where(r => r is BlockReducer).FirstOrDefault() as BlockReducer;

                    if (blockReducer is not null)
                        await blockReducer.RollbackBySlotAsync((ulong)rollbackEvent.RollBack.BlockSlot);
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
                            foreach (var reducerVariant in reducerVariants)
                            {
                                switch (reducerVariant)
                                {
                                    case OuraVariant.StakeDelegation:
                                        await reducer.HandleReduceAsync(_eventJson.Deserialize<OuraStakeDelegationEvent>(ConclaveJsonSerializerOptions));
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
                            // @TODO: Add Filter Logic
                            if (blockEvent.Context is not null) blockEvent.Context.InvalidTransactions = blockEvent.Block.InvalidTransactions;
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
                            await blockReducer.HandleReduceAsync(blockEvent);
                        }
                    }

                    if (blockEvent is null || blockEvent.Block is null || blockEvent.Block.Transactions is null) return Ok();

                    // Core Reducers
                    // @TODO: Cascade dbcontext foreach
                    await Task.WhenAll(_reducers.SelectMany(reducer =>
                    {
                        ICollection<OuraVariant> reducerVariants = _GetReducerVariants(reducer);
                        return reducerVariants.Select(reducerVariant =>
                        {
                            if (_settings.Value.Reducers.Any(rS => reducer.GetType().FullName?.Contains(rS) ?? false) && reducer is IOuraCoreReducer)
                            {
                                return reducerVariant switch
                                {
                                    OuraVariant.Block => reducer.HandleReduceAsync(blockEvent),
                                    OuraVariant.Transaction => Task.WhenAll(blockEvent.Block.Transactions.Select(te => reducer.HandleReduceAsync(te))),
                                    OuraVariant.TxInput => Task.WhenAll(blockEvent.Block.Transactions.SelectMany(t => t.Inputs!.ToList()).Select(i => reducer.HandleReduceAsync(i))),
                                    OuraVariant.TxOutput => Task.WhenAll(blockEvent.Block.Transactions.SelectMany(t => t.Outputs!.ToList()).Select(o => reducer.HandleReduceAsync(o))),
                                    OuraVariant.Asset => Task.WhenAll(MapToOuraAssetEvents(blockEvent.Block.Transactions.SelectMany(t => t.Outputs!)).ToList().Select(a => reducer.HandleReduceAsync(a))),
                                    OuraVariant.CollateralInput => Task.WhenAll(blockEvent.Block.Transactions.Where(t => t.CollateralInputs is not null).SelectMany(t => t.CollateralInputs!).Select(ci => reducer.HandleReduceAsync(ci))),
                                    OuraVariant.CollateralOutput => Task.WhenAll(blockEvent.Block.Transactions.Where(t => t.CollateralOutput is not null).Select(t => t.CollateralOutput!).Select(co => reducer.HandleReduceAsync(co))),
                                    _ => Task.CompletedTask
                                };
                            }
                            else
                            {
                                return Task.CompletedTask;
                            }
                        });
                    }));

                    // Other reducers in order of 

                    foreach (var transaction in blockEvent.Block.Transactions)
                    {
                        if (_reducers.Where(r => r is TransactionReducer).FirstOrDefault() is not TransactionReducer transactionReducer) continue;
                        await transactionReducer.HandleReduceAsync(transaction);

                        foreach (var reducer in _reducers)
                        {
                            if (_settings.Value.Reducers.Any(rS => reducer.GetType().FullName?.Contains(rS) ?? false) && reducer is not IOuraCoreReducer)
                            {
                                List<OuraVariant> reducerVariants = _GetReducerVariants(reducer).ToList();

                                foreach (var reducerVariant in reducerVariants)
                                {
                                    switch (reducerVariant)
                                    {
                                        case OuraVariant.Transaction:
                                            await reducer.HandleReduceAsync(transaction);
                                            break;
                                        case OuraVariant.TxInput:
                                            if (transaction.Inputs is null) break;
                                            foreach (var input in transaction.Inputs) await reducer.HandleReduceAsync(input);
                                            break;
                                        case OuraVariant.TxOutput:
                                            if (transaction.Outputs is null) break;
                                            foreach (var output in transaction.Outputs) await reducer.HandleReduceAsync(output);
                                            break;
                                        case OuraVariant.Asset:
                                            List<OuraAssetEvent> assets = MapToOuraAssetEvents(transaction.Outputs);
                                            foreach (var asset in assets) await reducer.HandleReduceAsync(asset);
                                            break;
                                        case OuraVariant.CollateralInput:
                                            if (transaction.CollateralInputs is null) break;
                                            foreach (var collateralInput in transaction.CollateralInputs) await reducer.HandleReduceAsync(collateralInput);
                                            break;
                                        case OuraVariant.CollateralOutput:
                                            if (!transaction.HasCollateralOutput) break;
                                            await reducer.HandleReduceAsync(transaction.CollateralOutput);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
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

}


