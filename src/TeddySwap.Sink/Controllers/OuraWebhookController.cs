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
    private readonly OuraService _ouraService;

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
        CardanoFilterService cardanoFilterService,
        OuraService ouraService
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
        _ouraService = ouraService;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveEventAsync([FromBody] JsonElement _eventJson)
    {
        OuraEvent? _event = _eventJson.Deserialize<OuraEvent>(ConclaveJsonSerializerOptions);

        if (_event is not null && _event.Context is not null)
        {
            using var blockDbContext = await _ouraService.CreateDbContextAsync(DbContextVariant.Core);
            if (_event.Variant == OuraVariant.RollBack)
            {
                OuraRollbackEvent? rollbackEvent = _eventJson.Deserialize<OuraRollbackEvent?>();
                if (rollbackEvent is not null && rollbackEvent.RollBack is not null && rollbackEvent.RollBack.BlockSlot is not null)
                {
                    _logger.LogInformation($"Rollback : Block Slot: {rollbackEvent.RollBack.BlockSlot}, Block Hash: {rollbackEvent.RollBack.BlockHash}");

                    BlockReducer? blockReducer = _reducers.Where(r => r is BlockReducer).FirstOrDefault() as BlockReducer;

                    if (blockReducer is not null)
                    {
                        await blockReducer.RollbackBySlotAsync((ulong)rollbackEvent.RollBack.BlockSlot);
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
                        List<OuraVariant> reducerVariants = _ouraService.GetReducerVariants(reducer).ToList();

                        if (_settings.Value.Reducers.Any(rS => reducer.GetType().FullName?.Contains(rS) ?? false) || reducer is IOuraCoreReducer)
                        {
                            using var reducerDbContext = await _ouraService.CreateDbContextAsync(_ouraService.GetDbContextVariant(reducer));
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
                            blockEvent = _ouraService.HydrateBlock(blockEvent);
                            await blockReducer.HandleReduceAsync(blockEvent, blockDbContext);
                        }
                    }

                    if (blockEvent is null || blockEvent.Block is null || blockEvent.Block.Transactions is null) return Ok();

                    List<OuraTransaction> transactions = _cardanoFilterService.FilterTransactions(blockEvent.Block.Transactions);
                    List<OuraTxInput> inputs = blockEvent.Block.Transactions.SelectMany(t => t.Inputs!).ToList();
                    List<OuraTxOutput> outputs = _cardanoFilterService.FilterTxOutputs(blockEvent.Block.Transactions.SelectMany(t => t.Outputs!).ToList());
                    List<OuraAssetEvent> assets = _cardanoFilterService.FilterAssets(_ouraService.MapToOuraAssetEvents(blockEvent.Block.Transactions.SelectMany(t => t.Outputs!)).ToList());
                    List<OuraTxInput> collateralInputs = blockEvent.Block.Transactions.Where(t => t.CollateralInputs is not null).SelectMany(t => t.CollateralInputs!).ToList();
                    List<OuraCollateralOutput> collateralOutputs = blockEvent.Block.Transactions.Where(t => t.CollateralOutput is not null).Select(t => t.CollateralOutput!).ToList();

                    IEnumerable<IOuraReducer> coreReducers = _reducers.Where(reducer => _settings.Value.Reducers.Any(rS => reducer.GetType().FullName?.Contains(rS) ?? false) && reducer is IOuraCoreReducer);
                    IEnumerable<IOuraReducer> otherReducers = _reducers.Where(reducer => _settings.Value.Reducers.Any(rS => reducer.GetType().FullName?.Contains(rS) ?? false) && reducer is not IOuraCoreReducer);

                    await _ouraService.HandleReducers(coreReducers, blockEvent, transactions, inputs, outputs, assets, collateralInputs, collateralOutputs);
                    await _ouraService.HandleReducers(otherReducers, blockEvent, transactions, inputs, outputs, assets, collateralInputs, collateralOutputs);

                    return Ok();
                }
            }
        }
        return Ok();
    }
}


