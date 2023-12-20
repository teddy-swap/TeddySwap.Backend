using Microsoft.EntityFrameworkCore;
using PallasDotnet.Models;
using TeddySwap.Data;
using TeddySwap.Data.Models.Reducers;

namespace TeddySwap.Sync.Reducers;

public class TeddyRewardClaimReqReducer(
    IDbContextFactory<TeddySwapDbContext> dbContextFactory,
    ILogger<TeddyYieldFarmingReducer> logger
) : IReducer
{
    private TeddySwapDbContext _dbContext = default!;
    private readonly ILogger<TeddyYieldFarmingReducer> _logger = logger;
    private readonly string _yieldRewardsWallet = "addr1q90n2rk4rurl3llmgq23ac5jw9lql8jgrn8p5a8cvv2hk8e642sq428m5mu0cemuc63spyr7nnn69tsh0lyrkqgnu38sn5efhm";
    private readonly string[] _tbcPolicies = [
        "ab182ed76b669b49ee54a37dee0d0064ad4208a859cc4fdf3f906d87",
        "da3562fad43b7759f679970fb4e0ec07ab5bebe5c703043acda07a3c",
    ];

    public async Task RollBackwardAsync(NextResponse response)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        _dbContext.RemoveRange(_dbContext.YieldClaimRequests.Where(r => r.Slot > response.Block.Slot));
        await _dbContext.SaveChangesAsync();
        _dbContext.Dispose();
    }

    public async Task RollForwardAsync(NextResponse response)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        foreach (var tx in response.Block.TransactionBodies)
        {
            var claimOutput = tx.Outputs.Where(o => o.Address.ToBech32() == _yieldRewardsWallet).FirstOrDefault();
            var txHasRewardClaim = claimOutput is not null;
            if (txHasRewardClaim)
            {
                var inputPairs = tx.Inputs.Select(i => i.Id.ToHex() + "_" + i.Index.ToString()).ToList();
                var resolvedInputs = await _dbContext.TransactionOutputs
                    .Where(o => inputPairs.Contains(o.Id + "_" + o.Index.ToString()))
                    .ToListAsync();

                var inputGroup = resolvedInputs.GroupBy(o => o.Address).Select(g => g.Key);

                if (inputGroup.Count() > 1)
                {
                    _logger.LogError("Invalid Claim: Multiple input addresses found for transaction {TxHash}", tx.Id.ToHex());
                    continue;
                }

                if(!inputGroup.Any())
                {
                    _logger.LogError("Invalid Claim: could not resolve input output {TxHash}", tx.Id.ToHex());
                    continue;
                }
                
                var claimAddress = inputGroup.First();

                var tbcs = claimOutput!.Amount.MultiAsset
                    .Where(
                        kvp => _tbcPolicies.Contains(kvp.Key.ToHex())
                    ).SelectMany(
                        kvp => kvp.Value.Select(kvp2 => $"{kvp.Key}.{kvp2.Key}")
                    ).ToArray();

                var minAda = tbcs.Length != 0 ? 5_000_000ul : 2_000_000ul;

                if (claimAddress != _yieldRewardsWallet && claimOutput!.Amount.Coin >= minAda)
                {
                    _dbContext.Add(new YieldClaimRequest(
                        claimAddress,
                        response.Block.Number,
                        response.Block.Slot,
                        tx.Id.ToHex(),
                        claimOutput!.Index,
                        tbcs,
                        null,
                        null,
                        null
                    ));
                }
            }
        }

        await _dbContext.SaveChangesAsync();
        _dbContext.Dispose();
    }
}