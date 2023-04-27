using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.Block)]
[DbContext(DbContextVariant.Core)]
public class BlockReducer : OuraReducerBase, IOuraCoreReducer
{
    private readonly ILogger<BlockReducer> _logger;
    private readonly CardanoService _cardanoService;
    private readonly IServiceProvider _serviceProvider;
    private readonly TeddySwapSinkSettings _settings;

    public BlockReducer(
        ILogger<BlockReducer> logger,
        CardanoService cardanoService,
        IOptions<TeddySwapSinkSettings> settings,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _cardanoService = cardanoService;
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
    }

    public async Task ReduceAsync(OuraBlockEvent blockEvent, TeddySwapSinkCoreDbContext _dbContext)
    {
        if (blockEvent.Context is not null &&
            blockEvent.Context.BlockNumber is not null &&
            blockEvent.Context.Slot is not null &&
            blockEvent.Context.BlockHash is not null &&
            blockEvent.Block is not null &&
            blockEvent.Block.Era is not null)
        {
            await RollbackBySlotAsync((ulong)blockEvent.Context.Slot, _dbContext);

            Block? existingBlock = await _dbContext.Blocks.Where(block => block.BlockNumber == blockEvent.Context.BlockNumber).FirstOrDefaultAsync();

            if (existingBlock is not null)
                _dbContext.Blocks.Remove(existingBlock);

            await _dbContext.Blocks.AddAsync(new()
            {
                BlockNumber = (ulong)blockEvent.Context.BlockNumber,
                VrfKeyhash = HashUtility.Blake2b256(blockEvent.Block.VrfVkey.HexToByteArray()).ToStringHex(),
                Slot = (ulong)blockEvent.Context.Slot,
                BlockHash = blockEvent.Context.BlockHash,
                Era = blockEvent.Block.Era,
                Epoch = _cardanoService.CalculateEpochBySlot((ulong)blockEvent.Context.Slot),
                InvalidTransactions = blockEvent.Block.InvalidTransactions
            });

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RollbackAsync(Block rollbackBlock, TeddySwapSinkCoreDbContext _dbContext)
    {
        _dbContext.Blocks.Remove(rollbackBlock);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RollbackBySlotAsync(ulong rollbackSlot, TeddySwapSinkCoreDbContext _dbContext)
    {
        ulong currentTipSlot = await _dbContext.Blocks.AnyAsync() ? await _dbContext.Blocks.MaxAsync(block => block.Slot) : 0;

        // Check if current database tip clashes with the current tip oura is pushing
        // if so then we should rollback and insert the new tip oura is pushing
        if (rollbackSlot < currentTipSlot)
        {
            List<Block> blocksToRollback = await _dbContext.Blocks
                .Where(block => block.Slot > rollbackSlot)
                .OrderByDescending(block => block.Slot)
                .ToListAsync();

            IEnumerable<IOuraReducer> reducers = _serviceProvider.GetServices<IOuraReducer>();

            foreach (Block rollbackBlock in blocksToRollback)
            {
                _logger.LogInformation($"Rolling back Block No: {rollbackBlock.BlockNumber}, Block Hash: {rollbackBlock.BlockHash}");

                await Task.WhenAll(reducers
                    .Where
                    (
                        reducer => reducer is not IOuraCoreReducer && _settings.Reducers.Any(rS => reducer.GetType().FullName?.Contains(rS) ?? false)
                    )
                    .Select((reducer) => Task.Run(async () =>
                    {
                        await reducer.HandleRollbackAsync(rollbackBlock, _dbContext);
                    }))
                );
                await this.HandleRollbackAsync(rollbackBlock, _dbContext);
            }
        }
    }
}