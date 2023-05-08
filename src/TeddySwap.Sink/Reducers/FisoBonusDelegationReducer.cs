using System.Text.Json;
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

[OuraReducer(OuraVariant.StakeDelegation)]
[DbContext(DbContextVariant.Fiso)]
public class FisoBonusDelegationReducer : OuraReducerBase
{
    private readonly CardanoService _cardanoService;
    private readonly TeddySwapSinkSettings _settings;

    public FisoBonusDelegationReducer(
        IOptions<TeddySwapSinkSettings> settings,
        CardanoService cardanoService)
    {
        _cardanoService = cardanoService;
        _settings = settings.Value;
    }

    public async Task ReduceAsync(OuraStakeDelegationEvent stakeDelegationEvent, TeddySwapFisoSinkDbContext _dbContext)
    {

        if (stakeDelegationEvent is not null &&
            stakeDelegationEvent.Context is not null &&
            stakeDelegationEvent.StakeDelegation is not null &&
            stakeDelegationEvent.StakeDelegation.Credential is not null &&
            stakeDelegationEvent.StakeDelegation.PoolHash is not null &&
            stakeDelegationEvent.Context.TxIdx is not null &&
            stakeDelegationEvent.Context.TxHash is not null &&
            stakeDelegationEvent.Context.BlockNumber is not null)
        {
            ulong epoch = _cardanoService.CalculateEpochBySlot((ulong)stakeDelegationEvent.Context.Slot!);

            if (epoch < _settings.FisoStartEpoch - 1 || epoch >= _settings.FisoEndEpoch) return;
            if (_cardanoService.IsInvalidTransaction(stakeDelegationEvent.Context.InvalidTransactions, (ulong)stakeDelegationEvent.Context.TxIdx)) return;

            string? stakeAddress = _cardanoService.GetStakeAddressFromEvent(stakeDelegationEvent);

            if (stakeAddress is null) return;

            string poolId = _cardanoService.PoolHashToBech32(stakeDelegationEvent.StakeDelegation.PoolHash);

            List<FisoPool> fisoPools = _settings.FisoPools
                .Where(fp => fp.JoinEpoch <= epoch)
                .ToList();

            if (!fisoPools.Select(fp => fp.PoolId).ToList().Contains(poolId)) return;

            var bonusFisoPool = await _dbContext.FisoPoolActiveStakes
                .Where(fpas => fpas.EpochNumber == epoch)
                .OrderBy(fpas => fpas.StakeAmount)
                .FirstOrDefaultAsync();

            if (bonusFisoPool is not null && poolId == bonusFisoPool.PoolId)
            {
                // check if stake address already has active bonus
                var delegatorBonus = await _dbContext.FisoBonusDelegations
                    .Where(fbd =>
                        fbd.StakeAddress == stakeAddress &&
                        fbd.PoolId == poolId &&
                        fbd.TxHash == stakeDelegationEvent.Context.TxHash &&
                        fbd.Slot == stakeDelegationEvent.Context.Slot)
                    .FirstOrDefaultAsync();

                if (delegatorBonus is not null) return;

                // create new entry
                await _dbContext.FisoBonusDelegations.AddAsync(new FisoBonusDelegation()
                {
                    EpochNumber = epoch,
                    StakeAddress = stakeAddress,
                    PoolId = poolId,
                    TxHash = stakeDelegationEvent.Context.TxHash,
                    Slot = (ulong)stakeDelegationEvent.Context.Slot,
                    BlockNumber = (ulong)stakeDelegationEvent.Context.BlockNumber
                });
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RollbackAsync(Block rollbackBlock, TeddySwapFisoSinkDbContext _dbContext)
    {
        var fisoBonusDelegations = await _dbContext.FisoBonusDelegations
            .Where(fbd => fbd.BlockNumber == rollbackBlock.BlockNumber)
            .ToListAsync();

        _dbContext.FisoBonusDelegations.RemoveRange(fisoBonusDelegations);
        await _dbContext.SaveChangesAsync();
    }
}