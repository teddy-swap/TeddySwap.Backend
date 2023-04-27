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

public class FisoRewardService
{
    private readonly ILogger<FisoRewardService> _logger;
    private readonly TeddySwapFisoSinkDbContext _dbContext;
    private readonly TeddySwapITNRewardSettings _settings;

    public FisoRewardService(
        ILogger<FisoRewardService> logger,
        TeddySwapFisoSinkDbContext dbContext,
        IOptions<TeddySwapITNRewardSettings> settings)
    {
        _logger = logger;
        _dbContext = dbContext;
        _settings = settings.Value;
    }

    public async Task<FisoRewardBreakdownResponse> GetFisoRewardBreakdownAsync(string stakeAddress)
    {
        List<FisoRewardResponse> rewards = await _dbContext.FisoEpochRewards
            .Where(fer => fer.StakeAddress == stakeAddress && fer.EpochNumber <= _settings.FisoEndEpoch)
            .OrderBy(fer => fer.EpochNumber)
            .Select(fer => new FisoRewardResponse()
            {
                PoolId = fer.PoolId,
                Epoch = fer.EpochNumber,
                ActiveStake = (ulong)fer.StakeAmount,
                BaseReward = (double)_settings.FisoRewardPerEpoch * (double)fer.SharePercentage,
                BonusReward = 0
            })
            .ToListAsync();

        ulong maxEpoch = await _dbContext.FisoPoolActiveStakes
            .Select(fpas => fpas.EpochNumber)
            .MaxAsync();

        List<ulong> epochsWithReward = new();

        foreach (string poolId in rewards.Select(r => r.PoolId).Distinct())
        {
            var bonusPoolDelegations = await _dbContext.FisoBonusDelegations
                .Where(fbd => fbd.StakeAddress == stakeAddress && fbd.PoolId == poolId)
                .OrderBy(r => r.Slot)
                .ToListAsync();

            var bonusPoolDelegation = bonusPoolDelegations
                .Where(bpd => rewards.Where(r => r.PoolId == bpd.PoolId).FirstOrDefault() != null)
                .OrderBy(bpd => bpd.Slot)
                .FirstOrDefault();

            if (bonusPoolDelegation is null) continue;

            List<FisoRewardResponse> filteredRewards = rewards
                .Where(r => r.PoolId == poolId && r.Epoch > bonusPoolDelegation.EpochNumber && r.ActiveStake > 0)
                .OrderBy(r => r.Epoch)
                .ToList();

            if (filteredRewards.Count <= 0) continue;

            epochsWithReward.AddRange(GroupConsecutiveEpochs(filteredRewards, maxEpoch));
        }

        rewards.ForEach(r =>
        {
            if (epochsWithReward.Contains(r.Epoch))
            {
                r.BonusReward = r.BaseReward * (double)0.25;
            }
        });

        return new()
        {
            FisoRewards = rewards,
            StakeAddress = stakeAddress,
            TotalBaseReward = rewards.Sum(r => r.BaseReward),
            TotalBonusReward = rewards.Sum(r => r.BonusReward)
        };
    }

    private List<ulong> GroupConsecutiveEpochs(IEnumerable<FisoRewardResponse> rewards, ulong currentMaxEpoch)
    {
        List<ulong> epochs = rewards.OrderBy(r => r.Epoch).Select(r => r.Epoch).ToList();
        List<List<ulong>> ranges = new();
        List<ulong> currentRange = new() { epochs[0] };
        ranges.Add(currentRange);

        for (int i = 1; i < epochs.Count; i++)
        {

            if (epochs[i] == epochs[i - 1] + 1)
            {
                currentRange.Add(epochs[i]);
            }
            else
            {
                currentRange = new List<ulong> { epochs[i] };
                ranges.Add(currentRange);
            }
        }

        List<List<ulong>> filteredRanges = ranges
            .Where(range =>
                {
                    ulong start = range.First();
                    ulong end = range.Last();
                    if (end == currentMaxEpoch)
                    {
                        end = _settings.FisoEndEpoch;
                    }
                    return end - start + 1 >= 6;
                })
            .ToList();

        List<ulong> flattenedEpochs = filteredRanges
            .SelectMany(range =>
            {
                ulong start = range.First();
                ulong end = range.Last();
                if (end == currentMaxEpoch)
                {
                    end = _settings.FisoEndEpoch;
                }
                return Enumerable.Range((int)start, (int)(end - start + 1)).Select(i => (ulong)i);
            })
            .Distinct()
            .ToList();

        return flattenedEpochs;
    }
}