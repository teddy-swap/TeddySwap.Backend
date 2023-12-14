using System.Numerics;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PallasDotnet.Models;
using TeddySwap.Data;
using TeddySwap.Data.Models.Reducers;

namespace TeddySwap.Sync.Reducers;

public class TeddyYieldFarmingReducer(
    IDbContextFactory<TeddySwapDbContext> dbContextFactory,
    ILogger<TeddyYieldFarmingReducer> logger,
    YieldFarmingDataService yieldFarmingDataService
) : IReducer
{

    private TeddySwapDbContext _dbContext = default!;
    private readonly ILogger<TeddyYieldFarmingReducer> _logger = logger;
    private readonly YieldFarmingDataService _yieldFarmingDataService = yieldFarmingDataService;

    const ulong YF_START_REWARD_AMOUNT = 178217820000;
    const ulong YF_MONTHLY_DECREASE = 3753950000;
    const int YF_TOTAL_MONTHS = 48;
    const ulong YF_SECONDS_IN_MONTH = 2592000;
    const long YF_SECONDS_IN_DAY = 86400;
    const ulong YF_START_SLOT = 109812631; // 1701378922 unix timestamp, 12/01/2023 5:15:22 AM UTC
    const long YF_START_TIME = 1701378922;
    const ulong MAX_LP_TOKENS = 9223372036854775807;

    private readonly string[] _lpTokens =
    [
        "0e59d5ce843858cf1574cc54d33d692d00df561b476f99c3731233e0.ADA_TEDY_LP",
        "18a44dde2d51a57964fedacc77182c45df88f86512c51e8f7eba0eb6.ADA_iBTC_LP",
        "ed8cc5ae2e5a68d78ecf333e86c466068242bbab2f8fca983a2f53e1.ADA_cBTC_LP",
        "b647e0f287bc8b664a55cea708af0064091447f2388dc508e3676a33.ADA_iUSD_LP",
        "672b7b2e1caa394f16d9efb23fb24b892fa5eab8156679da197d8c1c.ADA_CHRY_LP",
        "06aba05978dc7ef9a309ad6d55665cf4572ebd111dae22d1b241acaa.ADA_DJED_LP",
        "1f164eea5c242f53cb2df2150fa5ab7ba126350e904ddbcc65226e18.ADA_cNETA_LP",
        "0cd54b77ac0d70942895c7f1ebc8bdb06ec2fffbe1da6e26209675d2.ADA_FACT_LP",
        "c30d7086eeb68050a5b01efc219c5d4b5d5fd38e2e62fd6d7f01ac4d.ADA_LENFI_LP",
        "5d137c35eb5cba295aae2c44e0d9a82ca9f3d362caf3d681ffc9328b.ADA_ENCS_LP",
        "77e25ddcf2382e036d9d9416d894e15f0600886a4cfbac1b26ff7e03.ADA_SNEK_LP",
        "98de80cd7add6f1b9dacd076de508fc2cfad37d05b4dc6fbb8a510fa.ADA_iETH_LP",
        "ed3ea3cc3efda14d48d969e57ec22e2b3e5803ed4887c1152c48637c.ADA_INDY_LP",
        "3f241feae5f5cea28c3ea3b6746d7cdf76e4bae822c01e0b25ad2e38.ADA_OPTIM_LP"
    ];

    private readonly Dictionary<ulong, Dictionary<string, decimal>> _shareMap = new()
    {
        {
            109812631,
            new()
            {
                {"1c0ad45d50bd0a8c9bb851a9c59c3cb3e1ab2e2a29bd4d61b0e967ca.TEDY_ADA_POOL_IDENTITY", 0.6m},
                {"18a44dde2d51a57964fedacc77182c45df88f86512c51e8f7eba0eb6.iBTC_ADA_POOL_IDENTITY", 0.055m},
                {"ed8cc5ae2e5a68d78ecf333e86c466068242bbab2f8fca983a2f53e1.cBTC_ADA_POOL_IDENTITY", 0.055m},
                {"44de9976b4ef013ec683d49175f6edae92d1feeb2314fa18f060ea39.iUSD_ADA_POOL_IDENTITY", 0.055m},
                {"672b7b2e1caa394f16d9efb23fb24b892fa5eab8156679da197d8c1c.CHRY_ADA_POOL_IDENTITY", 0.055m},
                {"03a666d6ad004932bdd9d7e0d5a374262454cd84602bd494c9cd48d6.DJED_ADA_POOL_IDENTITY", 0.0225m},
                {"1f164eea5c242f53cb2df2150fa5ab7ba126350e904ddbcc65226e18.cNETA_ADA_POOL_IDENTITY", 0.0225m},
                {"0cd54b77ac0d70942895c7f1ebc8bdb06ec2fffbe1da6e26209675d2.FACT_ADA_POOL_IDENTITY", 0.0225m},
                {"c30d7086eeb68050a5b01efc219c5d4b5d5fd38e2e62fd6d7f01ac4d.AADA_ADA_POOL_IDENTITY",0.0225m},
                {"5d137c35eb5cba295aae2c44e0d9a82ca9f3d362caf3d681ffc9328b.ENCS_ADA_POOL_IDENTITY", 0.0225m},
                {"8d17d7a368cf5d1a3fe4468735050fdb8d2ae2bb2666aca05edd6969.SNEK_ADA_POOL_IDENTITY",0.0225m},
                {"98de80cd7add6f1b9dacd076de508fc2cfad37d05b4dc6fbb8a510fa.iETH_ADA_POOL_IDENTITY", 0.0225m},
                {"ed3ea3cc3efda14d48d969e57ec22e2b3e5803ed4887c1152c48637c.INDY_ADA_POOL_IDENTITY",0.0225m}
            }
        },
        {
            110393777,
            new()
            {
                {"1c0ad45d50bd0a8c9bb851a9c59c3cb3e1ab2e2a29bd4d61b0e967ca.TEDY_ADA_POOL_IDENTITY", 0.6m},
                {"18a44dde2d51a57964fedacc77182c45df88f86512c51e8f7eba0eb6.iBTC_ADA_POOL_IDENTITY", 0.055m},
                {"ed8cc5ae2e5a68d78ecf333e86c466068242bbab2f8fca983a2f53e1.cBTC_ADA_POOL_IDENTITY", 0.055m},
                {"44de9976b4ef013ec683d49175f6edae92d1feeb2314fa18f060ea39.iUSD_ADA_POOL_IDENTITY", 0.055m},
                {"672b7b2e1caa394f16d9efb23fb24b892fa5eab8156679da197d8c1c.CHRY_ADA_POOL_IDENTITY", 0.055m},
                {"03a666d6ad004932bdd9d7e0d5a374262454cd84602bd494c9cd48d6.DJED_ADA_POOL_IDENTITY", 0.02m},
                {"1f164eea5c242f53cb2df2150fa5ab7ba126350e904ddbcc65226e18.cNETA_ADA_POOL_IDENTITY", 0.02m},
                {"0cd54b77ac0d70942895c7f1ebc8bdb06ec2fffbe1da6e26209675d2.FACT_ADA_POOL_IDENTITY", 0.02m},
                {"c30d7086eeb68050a5b01efc219c5d4b5d5fd38e2e62fd6d7f01ac4d.AADA_ADA_POOL_IDENTITY", 0.02m},
                {"5d137c35eb5cba295aae2c44e0d9a82ca9f3d362caf3d681ffc9328b.ENCS_ADA_POOL_IDENTITY", 0.02m},
                {"8d17d7a368cf5d1a3fe4468735050fdb8d2ae2bb2666aca05edd6969.SNEK_ADA_POOL_IDENTITY", 0.02m},
                {"98de80cd7add6f1b9dacd076de508fc2cfad37d05b4dc6fbb8a510fa.iETH_ADA_POOL_IDENTITY", 0.02m},
                {"ed3ea3cc3efda14d48d969e57ec22e2b3e5803ed4887c1152c48637c.INDY_ADA_POOL_IDENTITY", 0.02m},
                {"3f241feae5f5cea28c3ea3b6746d7cdf76e4bae822c01e0b25ad2e38.OPTIM_ADA_POOL_IDENTITY", 0.02m}
            }
        }
    };

    private readonly Dictionary<string, string> _addressByPoolId = new()
    {
        {
            "1c0ad45d50bd0a8c9bb851a9c59c3cb3e1ab2e2a29bd4d61b0e967ca.TEDY_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8z9kp2avt5gp297dnxhxcmy6kkptepsr5pa409qa7gf8stzsxg8sx3"
        },
        {
            "18a44dde2d51a57964fedacc77182c45df88f86512c51e8f7eba0eb6.iBTC_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8z92v2k4gz85r5rq035n2llzemqvcz70h7hdr3njur05y6nsmrsjpe"
        },
        {
            "ed8cc5ae2e5a68d78ecf333e86c466068242bbab2f8fca983a2f53e1.cBTC_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8zxn5qy8sn2d7wtdtvjcsv7v0h7u9zsleljxv3nschr5sj3sla73t7"
        },
        {
            "44de9976b4ef013ec683d49175f6edae92d1feeb2314fa18f060ea39.iUSD_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8zrlxa5g3cwp6thfvzwhd9s4vcjjdwttsss65l09dum7g9rs0mr8px"
        },
        {
            "672b7b2e1caa394f16d9efb23fb24b892fa5eab8156679da197d8c1c.CHRY_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8z9kp2avt5gp297dnxhxcmy6kkptepsr5pa409qa7gf8stzsxg8sx3"
        },
        {
            "03a666d6ad004932bdd9d7e0d5a374262454cd84602bd494c9cd48d6.DJED_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8zp5hu6t748dfdd6cxlxxssyqez4wqwcrq44crfgkltqh2cqcwcjyr"
        },
        {
            "1f164eea5c242f53cb2df2150fa5ab7ba126350e904ddbcc65226e18.cNETA_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8z9kp2avt5gp297dnxhxcmy6kkptepsr5pa409qa7gf8stzsxg8sx3"
        },
        {
            "0cd54b77ac0d70942895c7f1ebc8bdb06ec2fffbe1da6e26209675d2.FACT_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8zz6mve63ntrqp7yxgkk395rngtzdmzdjzzuzdkdks0afwqsmdsegq"
        },
        {
            "c30d7086eeb68050a5b01efc219c5d4b5d5fd38e2e62fd6d7f01ac4d.AADA_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8zqgdzhkv23nm3v7tanurzu8v5vll365n7hq8f26937hatlqnv5cpz"
        },
        {
            "5d137c35eb5cba295aae2c44e0d9a82ca9f3d362caf3d681ffc9328b.ENCS_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8zzlsgmhduch9juwcjf6vjqeht0jv2g2mlz86wqh42h8akdqglnguu"
        },
        {
            "8d17d7a368cf5d1a3fe4468735050fdb8d2ae2bb2666aca05edd6969.SNEK_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8zr0vp2360e2j2gve54sxsheawjd6s6we2d25xl96a3r0jdqzvyqkl"
        },
        {
            "98de80cd7add6f1b9dacd076de508fc2cfad37d05b4dc6fbb8a510fa.iETH_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8zxk96389hhwyhv0t07gh89wqnaqg9cqkwsz4esd9sm562rs55tl66"
        },
        {
            "ed3ea3cc3efda14d48d969e57ec22e2b3e5803ed4887c1152c48637c.INDY_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8zphr7r6v67asj5jc5w5uapfapv0u9433m3v9aag9w46spaqc60ygw"
        },
        {
            "3f241feae5f5cea28c3ea3b6746d7cdf76e4bae822c01e0b25ad2e38.OPTIM_ADA_POOL_IDENTITY",
            "addr1zy5th50h46anh3v7zdvh7ve6amac7k4h3mdfvt0p6czm8z9re630pc4dzmhtku8276tyq0glgn53h93vw5rl9e6w4g8su86xvk"
        }
    };

    private readonly Dictionary<string, string> _poolLpByPoolId = new()
    {
        {
            "1c0ad45d50bd0a8c9bb851a9c59c3cb3e1ab2e2a29bd4d61b0e967ca.TEDY_ADA_POOL_IDENTITY",
            "0e59d5ce843858cf1574cc54d33d692d00df561b476f99c3731233e0.ADA_TEDY_LP"
        },
        {
            "18a44dde2d51a57964fedacc77182c45df88f86512c51e8f7eba0eb6.iBTC_ADA_POOL_IDENTITY",
            "18a44dde2d51a57964fedacc77182c45df88f86512c51e8f7eba0eb6.ADA_iBTC_LP"
        },
        {
            "ed8cc5ae2e5a68d78ecf333e86c466068242bbab2f8fca983a2f53e1.cBTC_ADA_POOL_IDENTITY",
            "ed8cc5ae2e5a68d78ecf333e86c466068242bbab2f8fca983a2f53e1.ADA_cBTC_LP"
        },
        {
            "44de9976b4ef013ec683d49175f6edae92d1feeb2314fa18f060ea39.iUSD_ADA_POOL_IDENTITY",
            "b647e0f287bc8b664a55cea708af0064091447f2388dc508e3676a33.ADA_iUSD_LP"
        },
        {
            "672b7b2e1caa394f16d9efb23fb24b892fa5eab8156679da197d8c1c.CHRY_ADA_POOL_IDENTITY",
            "672b7b2e1caa394f16d9efb23fb24b892fa5eab8156679da197d8c1c.ADA_CHRY_LP"
        },
        {
            "03a666d6ad004932bdd9d7e0d5a374262454cd84602bd494c9cd48d6.DJED_ADA_POOL_IDENTITY",
            "06aba05978dc7ef9a309ad6d55665cf4572ebd111dae22d1b241acaa.ADA_DJED_LP"
        },
        {
            "1f164eea5c242f53cb2df2150fa5ab7ba126350e904ddbcc65226e18.cNETA_ADA_POOL_IDENTITY",
            "1f164eea5c242f53cb2df2150fa5ab7ba126350e904ddbcc65226e18.ADA_cNETA_LP"
        },
        {
            "0cd54b77ac0d70942895c7f1ebc8bdb06ec2fffbe1da6e26209675d2.FACT_ADA_POOL_IDENTITY",
            "0cd54b77ac0d70942895c7f1ebc8bdb06ec2fffbe1da6e26209675d2.ADA_FACT_LP"
        },
        {
            "c30d7086eeb68050a5b01efc219c5d4b5d5fd38e2e62fd6d7f01ac4d.AADA_ADA_POOL_IDENTITY",
            "c30d7086eeb68050a5b01efc219c5d4b5d5fd38e2e62fd6d7f01ac4d.ADA_LENFI_LP"
        },
        {
            "5d137c35eb5cba295aae2c44e0d9a82ca9f3d362caf3d681ffc9328b.ENCS_ADA_POOL_IDENTITY",
            "5d137c35eb5cba295aae2c44e0d9a82ca9f3d362caf3d681ffc9328b.ADA_ENCS_LP"
        },
        {
            "8d17d7a368cf5d1a3fe4468735050fdb8d2ae2bb2666aca05edd6969.SNEK_ADA_POOL_IDENTITY",
            "77e25ddcf2382e036d9d9416d894e15f0600886a4cfbac1b26ff7e03.ADA_SNEK_LP"
        },
        {
            "98de80cd7add6f1b9dacd076de508fc2cfad37d05b4dc6fbb8a510fa.iETH_ADA_POOL_IDENTITY",
            "98de80cd7add6f1b9dacd076de508fc2cfad37d05b4dc6fbb8a510fa.ADA_iETH_LP"
        },
        {
            "ed3ea3cc3efda14d48d969e57ec22e2b3e5803ed4887c1152c48637c.INDY_ADA_POOL_IDENTITY",
            "ed3ea3cc3efda14d48d969e57ec22e2b3e5803ed4887c1152c48637c.ADA_INDY_LP"
        },
        {
            "3f241feae5f5cea28c3ea3b6746d7cdf76e4bae822c01e0b25ad2e38.OPTIM_ADA_POOL_IDENTITY",
            "3f241feae5f5cea28c3ea3b6746d7cdf76e4bae822c01e0b25ad2e38.ADA_OPTIM_LP"
        }
    };


    public async Task RollForwardAsync(NextResponse response)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        foreach (var tx in response.Block.TransactionBodies)
        {
            await ProcessInputsAsync(response.Block.Slot, response.Block.Number, tx.Inputs);
            await ProcessOutputsAsync(response.Block.Slot, response.Block.Number, tx.Outputs);
        }

        await RollForwardYieldFarmingRewardAsync(response);

        _dbContext.Dispose();
    }

    public async Task RollBackwardAsync(NextResponse response)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        var rollbackSlot = response.Block.Slot;
        _dbContext.LiquidityByAddress.RemoveRange(_dbContext.LiquidityByAddress.Where(lba => lba.Slot > rollbackSlot));
        await _dbContext.SaveChangesAsync();

        await RollBackwardYieldFarmingRewardAsync(response);

        _dbContext.Dispose();
    }

    public async Task RollForwardYieldFarmingRewardAsync(NextResponse response)
    {
        var lastDistribution = await _dbContext.YieldRewardByAddress.OrderByDescending(yr => yr.Slot).FirstOrDefaultAsync();
        var lastDistributionSlot = lastDistribution?.Slot ?? YF_START_SLOT;
        var lastDistributionTimestamp = lastDistribution?.Timestamp ?? DateTimeOffset.FromUnixTimeSeconds(YF_START_TIME);

        if (response.Block.Slot >= lastDistributionSlot + YF_SECONDS_IN_DAY)
        {
            var yfMonth = GetMonthFromSlot(response.Block.Slot, YF_START_SLOT);
            var dailyRewardAmount = GetDailyRewardAmount(yfMonth);
            var liquidity = await _yieldFarmingDataService.GetAllLiquidityAsync();
            var poolLiquidity = liquidity.Where(l => _addressByPoolId.ContainsValue(l.Address)).ToList();
            var userLiquidity = liquidity.Where(l => !_addressByPoolId.ContainsValue(l.Address)).ToList();
            var poolLockedLP = _addressByPoolId
                .ToDictionary(
                    pool => pool.Key,
                    pool =>
                    {
                        var assets = poolLiquidity
                        .Where(l => l.Address == pool.Value)
                        .FirstOrDefault()?
                        .Assets;
                        var lpUnit = _poolLpByPoolId[pool.Key].Split('.');
                        if (assets is not null && assets.TryGetValue(lpUnit[0], out Dictionary<string, ulong>? value))
                        {
                            var remainingLp = value[AsciiToHexString(lpUnit[1])];

                            return MAX_LP_TOKENS - remainingLp;
                        }
                        return 0UL;
                    }
                );

            var rewards = userLiquidity.SelectMany((ul) =>
            {
                var assets = ul.Assets;
                var userShare = _poolLpByPoolId.ToDictionary(
                    pool => pool.Key,
                    pool =>
                    {
                        var lpUnit = pool.Value.Split('.');
                        if (assets.TryGetValue(lpUnit[0], out Dictionary<string, ulong>? value))
                        {
                            if (value.ContainsKey(AsciiToHexString(lpUnit[1])))
                            {
                                var lockedLP = poolLockedLP[pool.Key];
                                if (lockedLP <= 0) return new { Share = 0m, LPAmount = 0UL };
                                var userLP = value[AsciiToHexString(lpUnit[1])];
                                return new { Share = (decimal)userLP / lockedLP, LPAmount = userLP };
                            }
                        }
                        return new { Share = 0m, LPAmount = 0UL };
                    }
                );

                var userRewards = userShare.Where(us => us.Value.Share > 0).Select(kvp =>
                {
                    var key = kvp.Key;
                    var shareMap = _shareMap.Where(sm => response.Block.Slot >= sm.Key).OrderByDescending(sm => sm.Key).FirstOrDefault().Value;
                    if (shareMap.TryGetValue(key, out decimal share))
                    {
                        var poolReward = dailyRewardAmount * share;
                        var userReward = poolReward * userShare[key].Share;
                        var lpAmount = userShare[key].LPAmount;
                        var yieldReward = new YieldRewardByAddress(
                            ul.Address,
                            key,
                            (ulong)(userReward * 1000000),
                            lpAmount,
                            userShare[key].Share,
                            false,
                            null,
                            response.Block.Number,
                            response.Block.Slot,
                            lastDistributionTimestamp.AddSeconds(YF_SECONDS_IN_DAY)
                        );
                        return yieldReward;
                    }
                    return null;
                });
                return userRewards;
            }).ToList();

            var totalRewards = rewards.Where(r => r is not null).Sum(r => (decimal)r!.Amount);
            if (totalRewards <= dailyRewardAmount)
            {
                _dbContext.AddRange(rewards.Where(r => r is not null).Select(r => r!));
                await _dbContext.SaveChangesAsync();
            }
            throw new Exception("Total rewards exceeds daily reward amount");
        }
    }

    public async Task RollBackwardYieldFarmingRewardAsync(NextResponse response)
    {
        _dbContext.RemoveRange(_dbContext.YieldRewardByAddress.Where(yr => yr.Slot > response.Block.Slot));
        await _dbContext.SaveChangesAsync();
    }

    private async Task ProcessInputsAsync(ulong slot, ulong blockNumber, IEnumerable<TransactionInput> inputs)
    {
        foreach (var input in inputs)
        {
            var resolvedInputOutput =
                _dbContext.TransactionOutputs.Local.Where(output => output.Id == input.Id.ToHex() && output.Index == input.Index).FirstOrDefault() ??
                await _dbContext.TransactionOutputs.Where(output => output.Id == input.Id.ToHex() && output.Index == input.Index).FirstOrDefaultAsync();

            if (resolvedInputOutput is not null)
            {
                bool hasUpdate = false;
                var address = resolvedInputOutput.Address;

                var lastLiquidityState =
                     _dbContext.LiquidityByAddress.Local.Where(lba => lba.Slot < slot && lba.Address == address).OrderByDescending(lba => lba.Slot).Take(1).FirstOrDefault() ??
                    await _dbContext.LiquidityByAddress.Where(lba => lba.Slot < slot && lba.Address == address).OrderByDescending(lba => lba.Slot).Take(1).FirstOrDefaultAsync();

                var assets = new Dictionary<string, Dictionary<string, ulong>>();
                var coins = 0UL;

                if (lastLiquidityState is not null)
                {
                    assets = lastLiquidityState.Assets;
                    coins = lastLiquidityState.Lovelace;
                }

                resolvedInputOutput.Amount.MultiAsset.ToList().ForEach(tokenBundle =>
                {
                    var policyId = tokenBundle.Key;

                    tokenBundle.Value.ToList().ForEach(token =>
                    {
                        // Check if policyId + token converted to ascii is in _lpTokens
                        var lpTokenHex = token.Key;
                        var lpToken = Encoding.ASCII.GetString(Convert.FromHexString(token.Key)).TrimEnd('\0');
                        var unit = $"{policyId}.{lpToken}";

                        if (_lpTokens.Contains(unit) && token.Value > 0)
                        {
                            hasUpdate = true;

                            if (assets[policyId].ContainsKey(lpTokenHex))
                            {
                                assets[policyId][lpTokenHex] -= token.Value;
                            }
                        }
                    });
                });

                var thisBlockLiquidityState =
                    _dbContext.LiquidityByAddress.Local.Where(lba => lba.Slot == slot && lba.Address == address).FirstOrDefault() ??
                    await _dbContext.LiquidityByAddress.Where(lba => lba.Slot == slot && lba.Address == address).FirstOrDefaultAsync();

                if (assets.Count > 0 && hasUpdate)
                {
                    if (thisBlockLiquidityState is null)
                    {
                        thisBlockLiquidityState = new() { Address = address, Slot = slot, BlockNumber = blockNumber, Assets = assets, Lovelace = coins };
                        thisBlockLiquidityState = _dbContext.LiquidityByAddress.Add(thisBlockLiquidityState).Entity;
                    }
                    else
                    {
                        thisBlockLiquidityState.Assets = assets;
                    }
                }

                // Update Lovelace for Liquidity Pools Only
                if (_addressByPoolId.ContainsValue(address))
                {
                    if (thisBlockLiquidityState is not null)
                    {
                        thisBlockLiquidityState.Lovelace -= resolvedInputOutput.Amount.Coin;
                    }
                    else
                    {
                        thisBlockLiquidityState = new() { Address = address, Slot = slot, BlockNumber = blockNumber, Assets = assets, Lovelace = coins - resolvedInputOutput.Amount.Coin };
                        _dbContext.LiquidityByAddress.Add(thisBlockLiquidityState);
                    }
                }
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task ProcessOutputsAsync(ulong slot, ulong blockNumber, IEnumerable<TransactionOutput> outputs)
    {
        foreach (var utxo in outputs)
        {
            bool hasAssetUpdate = false;
            var address = utxo.Address.ToBech32();

            var lastLiquidityState = _dbContext.LiquidityByAddress.Local.Where(lba => lba.Slot < slot && lba.Address == address).OrderByDescending(lba => lba.Slot).Take(1).FirstOrDefault() ??
                await _dbContext.LiquidityByAddress.Where(lba => lba.Slot < slot && lba.Address == address).OrderByDescending(lba => lba.Slot).Take(1).FirstOrDefaultAsync();

            var assets = new Dictionary<string, Dictionary<string, ulong>>();
            var coins = 0UL;

            if (lastLiquidityState is not null)
            {
                assets = lastLiquidityState.Assets;
                coins = lastLiquidityState.Lovelace;
            }

            utxo.Amount.MultiAsset.ToList().ForEach(tokenBundle =>
            {
                var policyId = tokenBundle.Key.ToHex();

                tokenBundle.Value.ToList().ForEach(token =>
                {
                    // Check if policyId + token converted to ascii is in _lpTokens
                    var lpTokenHex = token.Key.ToHex();
                    var lpToken = Encoding.ASCII.GetString(token.Key.Bytes).TrimEnd('\0');
                    var unit = $"{policyId}.{lpToken}";

                    if (_lpTokens.Contains(unit) && token.Value > 0)
                    {
                        hasAssetUpdate = true;

                        if (!assets.ContainsKey(policyId))
                        {
                            assets.Add(policyId, []);
                        }

                        if (!assets[policyId].ContainsKey(lpTokenHex))
                        {
                            assets[policyId].Add(lpTokenHex, token.Value);
                        }
                        else
                        {
                            assets[policyId][lpTokenHex] += token.Value;
                        }
                    }
                });
            });

            var thisBlockLiquidityState =
                _dbContext.LiquidityByAddress.Local.Where(lba => lba.Slot == slot && lba.Address == address).FirstOrDefault() ??
                await _dbContext.LiquidityByAddress.Where(lba => lba.Slot == slot && lba.Address == address).FirstOrDefaultAsync();

            if (assets.Count > 0 && hasAssetUpdate)
            {
                if (thisBlockLiquidityState is null)
                {
                    thisBlockLiquidityState = new() { Address = address, Slot = slot, BlockNumber = blockNumber, Assets = assets, Lovelace = coins };
                    _dbContext.LiquidityByAddress.Add(thisBlockLiquidityState);
                }
                else
                {
                    thisBlockLiquidityState.Assets = assets;
                }
            }

            // Update Lovelace for Liquidity Pools Only
            if (_addressByPoolId.ContainsValue(address))
            {
                if (thisBlockLiquidityState is not null)
                {
                    thisBlockLiquidityState.Lovelace += utxo.Amount.Coin;
                }
                else
                {
                    thisBlockLiquidityState = new() { Address = address, Slot = slot, BlockNumber = blockNumber, Assets = assets, Lovelace = coins + utxo.Amount.Coin };
                    _dbContext.LiquidityByAddress.Add(thisBlockLiquidityState);
                }
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    public static (int Month, decimal MonthlyDistribution, decimal CumulativeDistribution) GetMonthlyRewards(int month)
    {
        if (month < 1 || month > YF_TOTAL_MONTHS)
        {
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and " + YF_TOTAL_MONTHS);
        }

        ulong monthlyReward = YF_START_REWARD_AMOUNT - YF_MONTHLY_DECREASE * (ulong)(month - 1);
        ulong cumulativeReward = 0;

        for (int i = 1; i <= month; i++)
        {
            cumulativeReward += YF_START_REWARD_AMOUNT - YF_MONTHLY_DECREASE * (ulong)(i - 1);
        }

        return (
            month,
            monthlyReward / (decimal)1000000,
            cumulativeReward / (decimal)1000000
        );
    }

    public static int GetMonthFromSlot(ulong slot, ulong startSlot)
    {
        // Calculate the time difference in slots
        ulong timeDifference = slot - startSlot;

        // Convert to seconds
        ulong timeDifferenceInSeconds = timeDifference * YF_SECONDS_IN_DAY;

        // Calculate the elapsed days
        int elapsedDays = Convert.ToInt32(timeDifferenceInSeconds / YF_SECONDS_IN_DAY);

        // Determine the month
        return (int)Math.Ceiling((decimal)elapsedDays / YF_SECONDS_IN_MONTH);
    }

    private decimal GetDailyRewardAmount(int month)
    {
        var (_, MonthlyDistribution, _) = GetMonthlyRewards(month);
        return MonthlyDistribution / 30;
    }

    private static string HexStringToAscii(string hexString)
    {
        var data = Convert.FromHexString(hexString);
        return Encoding.ASCII.GetString(data);
    }

    private static string AsciiToHexString(string asciiString)
    {
        var data = Encoding.ASCII.GetBytes(asciiString);
        return Convert.ToHexString(data).ToLowerInvariant();
    }
}