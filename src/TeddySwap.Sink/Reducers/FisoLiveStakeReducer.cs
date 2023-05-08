using System.Text.Json;
using CardanoSharp.Koios.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.TxInput, OuraVariant.TxOutput, OuraVariant.CollateralInput, OuraVariant.CollateralOutput)]
[DbContext(DbContextVariant.Fiso)]
public class FisoLiveStakeReducer : OuraReducerBase
{
    private readonly IDbContextFactory<CardanoDbSyncContext> _cardanoDbSyncContextFactory;
    private readonly CardanoService _cardanoService;

    private readonly TeddySwapSinkSettings _settings;

    public FisoLiveStakeReducer(
        IOptions<TeddySwapSinkSettings> settings,
        CardanoService cardanoService,
        IDbContextFactory<CardanoDbSyncContext> cardanoDbSyncContextFactory)
    {
        _cardanoService = cardanoService;
        _settings = settings.Value;
        _cardanoDbSyncContextFactory = cardanoDbSyncContextFactory;
    }

    public async Task ReduceAsync(OuraEvent ouraEvent, TeddySwapFisoSinkDbContext _dbContext)
    {
        if (ouraEvent.Context is null || ouraEvent.Context.Slot is null) return;

        ulong epoch = _cardanoService.CalculateEpochBySlot((ulong)ouraEvent.Context.Slot);

        if (epoch < _settings.FisoStartEpoch - 1 || epoch >= _settings.FisoEndEpoch) return;
        await (ouraEvent.Variant switch
        {
            OuraVariant.TxInput => Task.Run(async () =>
            {
                OuraTxInput? txInput = ouraEvent as OuraTxInput;
                if (txInput is not null &&
                    txInput.Context is not null &&
                    txInput.Context.Slot is not null &&
                    txInput.Context.TxIdx is not null &&
                    txInput.Context.TxHash is not null)
                {
                    if (_cardanoService.IsInvalidTransaction(txInput.Context.InvalidTransactions, (ulong)txInput.Context.TxIdx)) return;

                    TxOutput? input = await _dbContext.TxOutputs
                        .Where(txOut => txOut.TxHash == txInput.TxHash && txOut.Index == txInput.Index)
                        .FirstOrDefaultAsync();

                    if (input is null)
                    {
                        var txOut = await GetTxOut(txInput.TxHash, (int)txInput.Index);

                        if (txOut is null) return;

                        input = new TxOutput()
                        {
                            Address = txOut.Address,
                            Amount = (ulong)txOut.Value,
                        };
                    }

                    if (input is not null)
                    {
                        string? stakeAddress = _cardanoService.TryGetStakeAddress(input.Address);
                        if (stakeAddress is not null)
                        {
                            var fisoDelegator = await _dbContext.FisoDelegators.Where(fd => fd.StakeAddress == stakeAddress && fd.Epoch == epoch).FirstOrDefaultAsync();

                            if (fisoDelegator is not null)
                            {
                                var fisoPoolActiveStake = await _dbContext.FisoPoolActiveStakes
                                    .Where(fpas => fpas.PoolId == fisoDelegator.PoolId && fpas.EpochNumber == epoch)
                                    .FirstOrDefaultAsync();

                                if (fisoPoolActiveStake is not null)
                                {
                                    fisoPoolActiveStake.StakeAmount -= input.Amount;
                                    _dbContext.FisoPoolActiveStakes.Update(fisoPoolActiveStake);
                                }
                            }
                        }
                    }
                    await _dbContext.SaveChangesAsync();

                }
            }),
            OuraVariant.TxOutput => Task.Run(async () =>
            {
                OuraTxOutput? txOutput = ouraEvent as OuraTxOutput;
                if (txOutput is not null &&
                    txOutput.Amount is not null &&
                    txOutput.Address is not null &&
                    txOutput.Context is not null &&
                    txOutput.Context.Slot is not null &&
                    txOutput.Context.TxIdx is not null)
                {
                    if (txOutput.Context.InvalidTransactions is not null &&
                        txOutput.Context.InvalidTransactions.ToList().Contains((ulong)txOutput.Context.TxIdx)) return;

                    string? stakeAddress = _cardanoService.TryGetStakeAddress(txOutput.Address);
                    ulong amount = (ulong)txOutput.Amount;


                    var fisoDelegator = await _dbContext.FisoDelegators.Where(fd => fd.StakeAddress == stakeAddress && fd.Epoch == epoch).FirstOrDefaultAsync();

                    if (fisoDelegator is not null)
                    {
                        var fisoPoolActiveStake = await _dbContext.FisoPoolActiveStakes
                            .Where(fpas => fpas.PoolId == fisoDelegator.PoolId && fpas.EpochNumber == epoch)
                            .FirstOrDefaultAsync();

                        if (fisoPoolActiveStake is not null)
                        {
                            fisoPoolActiveStake.StakeAmount += amount;
                            _dbContext.FisoPoolActiveStakes.Update(fisoPoolActiveStake);
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }),
            OuraVariant.CollateralInput => Task.Run(async () =>
            {
                OuraTxInput? txInput = ouraEvent as OuraTxInput;

                if (txInput is not null &&
                    txInput.Context is not null &&
                    txInput.Context.Slot is not null &&
                    txInput.Context.TxIdx is not null)
                {
                    if (txInput.Context.HasCollateralOutput && txInput.Context.TxHash is not null)
                    {
                        TxOutput? input = await _dbContext.TxOutputs
                            .Where(txOut => txOut.TxHash == txInput.TxHash && txOut.Index == txInput.Index)
                            .FirstOrDefaultAsync();

                        if (input is null)
                        {
                            var txOut = await GetTxOut(txInput.TxHash, (int)txInput.Index);

                            if (txOut is null) return;

                            input = new TxOutput()
                            {
                                Address = txOut.Address,
                                Amount = (ulong)txOut.Value,
                            };
                        }

                        if (input is not null)
                        {
                            string? stakeAddress = _cardanoService.TryGetStakeAddress(input.Address);
                            if (stakeAddress is not null)
                            {
                                var fisoDelegator = await _dbContext.FisoDelegators.Where(fd => fd.StakeAddress == stakeAddress && fd.Epoch == epoch).FirstOrDefaultAsync();

                                if (fisoDelegator is not null)
                                {
                                    var fisoPoolActiveStake = await _dbContext.FisoPoolActiveStakes
                                        .Where(fpas => fpas.PoolId == fisoDelegator.PoolId && fpas.EpochNumber == epoch)
                                        .FirstOrDefaultAsync();

                                    if (fisoPoolActiveStake is not null)
                                    {
                                        fisoPoolActiveStake.StakeAmount -= input.Amount;
                                        _dbContext.FisoPoolActiveStakes.Update(fisoPoolActiveStake);
                                    }
                                }
                            }
                        }
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }),
            OuraVariant.CollateralOutput => Task.Run(async () =>
            {
                OuraCollateralOutput? txOutput = ouraEvent as OuraCollateralOutput;
                if (txOutput is not null &&
                    txOutput.Context is not null &&
                    txOutput.Context.HasCollateralOutput &&
                    txOutput.Context.Slot is not null &&
                    txOutput.Address is not null)
                {
                    string? stakeAddress = _cardanoService.TryGetStakeAddress(txOutput.Address);
                    ulong amount = txOutput.Amount;

                    var fisoDelegator = await _dbContext.FisoDelegators.Where(fd => fd.StakeAddress == stakeAddress && fd.Epoch == epoch).FirstOrDefaultAsync();

                    if (fisoDelegator is not null)
                    {
                        var fisoPoolActiveStake = await _dbContext.FisoPoolActiveStakes
                            .Where(fpas => fpas.PoolId == fisoDelegator.PoolId && fpas.EpochNumber == epoch)
                            .FirstOrDefaultAsync();

                        if (fisoPoolActiveStake is not null)
                        {
                            fisoPoolActiveStake.StakeAmount += amount;
                            _dbContext.FisoPoolActiveStakes.Update(fisoPoolActiveStake);
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }),
            _ => Task.Run(() => { })
        });
    }

    private async Task<Common.Models.CardanoDbSync.TxOut?> GetTxOut(string hash, int index)
    {
        using CardanoDbSyncContext _dbContext = await _cardanoDbSyncContextFactory.CreateDbContextAsync();
        byte[] txHashBytes = Convert.FromHexString(hash);
        Common.Models.CardanoDbSync.TxOut? txOut = await _dbContext.TxOuts
            .Include(to => to.Tx)
            .Where(to => to.Tx.Hash == txHashBytes && to.Index == index)
            .FirstOrDefaultAsync();

        return txOut;
    }

    private async Task<TxOutput?> ResolveTxOutAsync(string hash, ulong index, TeddySwapFisoSinkDbContext dbContext)
    {
        TxOutput? txInOut = await dbContext.TxOutputs
            .Where(txOut => txOut.TxHash == hash && txOut.Index == index)
            .FirstOrDefaultAsync();

        if (txInOut is null)
        {
            var txOut = await GetTxOut(hash, (int)index);

            if (txOut is null) return null;

            txInOut = new TxOutput()
            {
                Address = txOut.Address,
                Amount = (ulong)txOut.Value,
            };
        }

        return txInOut;
    }

    private async Task UpdateFisoPoolAsync(string address, ulong epoch, ulong amount, TeddySwapFisoSinkDbContext dbContext)
    {
        string? stakeAddress = _cardanoService.TryGetStakeAddress(address);

        if (stakeAddress is null) return;

        var fisoDelegator = await dbContext.FisoDelegators
            .Where(fd => fd.StakeAddress == stakeAddress && fd.Epoch == epoch)
            .FirstOrDefaultAsync();

        if (fisoDelegator is null) return;

        var fisoPoolActiveStake = await dbContext.FisoPoolActiveStakes
            .Where(fpas => fpas.PoolId == fisoDelegator.PoolId && fpas.EpochNumber == epoch)
            .FirstOrDefaultAsync();

        if (fisoPoolActiveStake is null) return;

        fisoPoolActiveStake.StakeAmount += amount;
        dbContext.FisoPoolActiveStakes.Update(fisoPoolActiveStake);
    }

    private async Task UpdateFisoPoolCollaterals(Transaction transaction, ulong epoch, TeddySwapFisoSinkDbContext dbContext)
    {
        var collateralInputs = await dbContext.CollateralTxIns
                    .Where(ci => ci.TxHash == transaction.Hash)
                    .ToListAsync();

        var collateralTxOut = await dbContext.CollateralTxOuts
            .Where(co => co.TxHash == transaction.Hash)
            .FirstOrDefaultAsync();

        foreach (CollateralTxIn collateralTxIn in collateralInputs)
        {
            TxOutput? txInOut = await ResolveTxOutAsync(collateralTxIn.TxHash, collateralTxIn.TxOutputIndex, dbContext);

            if (txInOut is null) continue;

            await UpdateFisoPoolAsync(txInOut.Address, epoch, txInOut.Amount, dbContext);
        }

        if (collateralTxOut is not null) await UpdateFisoPoolAsync(collateralTxOut.Address, epoch, collateralTxOut.Amount, dbContext);

    }

    public async Task RollbackAsync(Block rollbackBlock, TeddySwapFisoSinkDbContext _dbContext)
    {
        var transactions = await _dbContext.Transactions
            .Where(t => t.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        foreach (Transaction transaction in transactions)
        {
            ulong epoch = rollbackBlock.Epoch;

            if (transaction.IsValid)
            {
                var inputs = await _dbContext.TxInputs
                    .Where(i => i.TxHash == transaction.Hash)
                    .ToListAsync();

                var outputs = await _dbContext.TxOutputs
                    .Where(o => o.TxHash == transaction.Hash)
                    .ToListAsync();

                foreach (TxInput input in inputs)
                {
                    TxOutput? txInOut = await ResolveTxOutAsync(input.TxHash, input.TxOutputIndex, _dbContext);

                    if (txInOut is null) continue;

                    await UpdateFisoPoolAsync(txInOut.Address, epoch, txInOut.Amount, _dbContext);
                }

                foreach (TxOutput output in outputs) await UpdateFisoPoolAsync(output.Address, epoch, output.Amount, _dbContext);
                if (transaction.HasCollateralOutput) await UpdateFisoPoolCollaterals(transaction, epoch, _dbContext);
            }
            else
            {
                await UpdateFisoPoolCollaterals(transaction, epoch, _dbContext);
            }
        }
    }
}