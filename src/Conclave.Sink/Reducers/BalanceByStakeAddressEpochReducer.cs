using Conclave.Sink.Data;
using Conclave.Sink.Models;
using CardanoSharp.Wallet.Models.Addresses;
using Microsoft.EntityFrameworkCore;
using CardanoSharp.Wallet.Extensions.Models;
using Conclave.Sink.Services;

namespace Conclave.Sink.Reducers;

[OuraReducer(OuraVariant.TxInput, OuraVariant.TxOutput)]
public class BalanceByStakeAddressEpochReducer : OuraReducerBase
{
    private readonly ILogger<BalanceByStakeAddressEpoch> _logger;
    private readonly IDbContextFactory<ConclaveSinkDbContext> _dbContextFactory;
    private readonly CardanoService _cardanoService;

    public BalanceByStakeAddressEpochReducer(
        ILogger<BalanceByStakeAddressEpoch> logger,
        IDbContextFactory<ConclaveSinkDbContext> dbContextFactory,
        CardanoService cardanoService)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _cardanoService = cardanoService;
    }

    public async Task ReduceAsync(OuraEvent ouraEvent)
    {
        using ConclaveSinkDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();
        await (ouraEvent.Variant switch
        {
            OuraVariant.TxInput => Task.Run(async () =>
            {
                OuraTxInputEvent? txInputEvent = ouraEvent as OuraTxInputEvent;
                if (txInputEvent is not null &&
                    txInputEvent.TxInput is not null &&
                    txInputEvent.Context is not null &&
                    txInputEvent.Context.Slot is not null)
                {
                    TxOutput? input = await _dbContext.TxOutputs
                        .Include(txOut => txOut.Transaction)
                        .ThenInclude(transaction => transaction.Block)
                        .Where(txOut => (txOut.TxHash == txInputEvent.TxInput.TxHash) && (txOut.Index == txInputEvent.TxInput.Index))
                        .FirstOrDefaultAsync();

                    if (input is not null)
                    {
                        Address? stakeAddress = TryGetStakeAddress(new Address(input.Address));
                        ulong epoch = _cardanoService.CalculateEpochBySlot((ulong)txInputEvent.Context.Slot);

                        if (stakeAddress is null) return;

                        BalanceByStakeAddressEpoch? entry = await _dbContext.BalanceByStakeAddressEpoches
                            .Where((bbae) => (bbae.StakeAddress == stakeAddress.ToString()) && (bbae.Epoch == epoch))
                            .FirstOrDefaultAsync();

                        if (entry is not null)
                        {
                            entry.Balance -= input.Amount;
                        }
                        else
                        {
                            ulong lastEpochBalance = await GetLastEpochBalanceByStakeAddressAsync(stakeAddress.ToString(), epoch);
                            ulong balance = lastEpochBalance - input.Amount;

                            await _dbContext.BalanceByStakeAddressEpoches.AddAsync(new()
                            {
                                StakeAddress = stakeAddress.ToString(),
                                Balance = balance,
                                Epoch = epoch
                            });
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }),
            OuraVariant.TxOutput => Task.Run(async () =>
            {
                OuraTxOutputEvent? txOutputEvent = ouraEvent as OuraTxOutputEvent;
                if (txOutputEvent is not null &&
                    txOutputEvent.TxOutput is not null &&
                    txOutputEvent.TxOutput.Amount is not null &&
                    txOutputEvent.TxOutput.Address is not null &&
                    txOutputEvent.Context is not null &&
                    txOutputEvent.Context.Slot is not null)
                {
                    Address? stakeAddress = TryGetStakeAddress(new Address(txOutputEvent.TxOutput.Address));
                    ulong epoch = _cardanoService.CalculateEpochBySlot((ulong)txOutputEvent.Context.Slot);
                    ulong amount = (ulong)txOutputEvent.TxOutput.Amount;

                    if (stakeAddress is null) return;

                    BalanceByStakeAddressEpoch? entry = await _dbContext.BalanceByStakeAddressEpoches
                        .Where((bbae) => (bbae.StakeAddress == stakeAddress.ToString()) && (bbae.Epoch == epoch))
                        .FirstOrDefaultAsync();

                    if (entry is not null)
                    {
                        entry.Balance += amount;
                    }
                    else
                    {
                        ulong previousBalance = await GetLastEpochBalanceByStakeAddressAsync(stakeAddress.ToString(), epoch);
                        ulong balance = previousBalance + amount;

                        await _dbContext.BalanceByStakeAddressEpoches.AddAsync(new()
                        {
                            StakeAddress = stakeAddress.ToString(),
                            Balance = balance,
                            Epoch = epoch
                        });
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }),
            _ => Task.Run(() => { })
        });
    }

    private Address? TryGetStakeAddress(Address paymentAddress)
    {
        try
        {
            return paymentAddress.GetStakeAddress();
        }
        catch { }
        return null;
    }

    public async Task<ulong> GetLastEpochBalanceByStakeAddressAsync(string stakeAddress, ulong? epoch)
    {
        using ConclaveSinkDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await _dbContext.BalanceByStakeAddressEpoches
            .Where((bbae) => (bbae.StakeAddress == stakeAddress && bbae.Epoch < epoch))
            .OrderByDescending(s => s.Epoch)
            .Select(s => s.Balance)
            .FirstOrDefaultAsync();
    }

    public async Task RollbackAsync(Block rollbackBlock)
    {
        using ConclaveSinkDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

        IEnumerable<TxInput> consumed = await _dbContext.TxInputs
            .Include(txInput => txInput.Transaction)
            .ThenInclude(transaction => transaction.Block)
            .Include(txInput => txInput.TxOutput)
            .Where(txInput => txInput.Transaction.Block == rollbackBlock)
            .ToListAsync();

        IEnumerable<TxOutput> produced = await _dbContext.TxOutputs
            .Where(txOutput => txOutput.Transaction.Block == rollbackBlock)
            .ToListAsync();

        IEnumerable<Task> consumeTasks = consumed.ToList().Select(txInput => Task.Run(async () =>
        {
            Address? stakeAddress = TryGetStakeAddress(new Address(txInput.TxOutput.Address));

            if (stakeAddress is not null)
            {
                BalanceByStakeAddressEpoch? entry = await _dbContext.BalanceByStakeAddressEpoches
                    .Where((bbsae) => (bbsae.StakeAddress == stakeAddress.ToString()) && (bbsae.Epoch == rollbackBlock.Epoch))
                    .FirstOrDefaultAsync();

                if (entry is not null)
                {
                    entry.Balance += txInput.TxOutput.Amount;
                }
            }
        }));

        foreach (Task consumeTask in consumeTasks) await consumeTask;

        IEnumerable<Task> produceTasks = produced.ToList().Select(txOutput => Task.Run(async () =>
        {
            Address? stakeAddress = TryGetStakeAddress(new Address(txOutput.Address));

            if (stakeAddress is not null)
            {
                BalanceByStakeAddressEpoch? entry = await _dbContext.BalanceByStakeAddressEpoches
                    .Where((bba) => (bba.StakeAddress == stakeAddress.ToString()) && (bba.Epoch == rollbackBlock.Epoch))
                    .FirstOrDefaultAsync();

                if (entry is not null)
                {
                    ulong previousBalance = await GetLastEpochBalanceByStakeAddressAsync(stakeAddress.ToString(), rollbackBlock.Epoch);

                    entry.Balance -= txOutput.Amount;

                    if (entry.Balance <= 0 || entry.Balance <= previousBalance)
                    {
                        _dbContext.Remove(entry);
                    }
                }
            };
        }));

        foreach (Task produceTask in produceTasks) await produceTask;

        await _dbContext.SaveChangesAsync();
    }
}