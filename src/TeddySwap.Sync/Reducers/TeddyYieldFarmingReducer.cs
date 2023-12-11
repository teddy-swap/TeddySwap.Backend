using System.Text;
using Microsoft.EntityFrameworkCore;
using PallasDotnet.Models;
using TeddySwap.Data;
using TeddySwap.Data.Models.Reducers;

namespace TeddySwap.Sync.Reducers;

public class TeddyYieldFarmingReducer(IDbContextFactory<TeddySwapDbContext> dbContextFactory, ILogger<TeddyYieldFarmingReducer> logger) : IReducer
{
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

    private readonly Dictionary<string, decimal> _shareMap = new()
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
    };

    private TeddySwapDbContext _dbContext = default!;
    private readonly ILogger<TeddyYieldFarmingReducer> _logger = logger;

    public async Task RollForwardAsync(NextResponse response)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        foreach (var tx in response.Block.TransactionBodies)
        {
            await ProcessInputsAsync(response.Block.Slot, response.Block.Number, tx.Inputs);
            await ProcessOutputsAsync(response.Block.Slot, response.Block.Number, tx.Outputs);
        }
        _dbContext.Dispose();
    }

    public async Task RollBackwardAsync(NextResponse response)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        var rollbackSlot = response.Block.Slot;
        _dbContext.LiquidityByAddress.RemoveRange(_dbContext.LiquidityByAddress.Where(lba => lba.Slot > rollbackSlot));
        await _dbContext.SaveChangesAsync();
        _dbContext.Dispose();
    }

    private async Task ProcessInputsAsync(ulong slot, ulong blockNumber, IEnumerable<TransactionInput> inputs)
    {
        foreach (var input in inputs)
        {
            var resolvedInputOutput = await _dbContext.TransactionOutputs
                .Where(output => output.Id == input.Id.ToHex() && output.Index == input.Index)
                .FirstOrDefaultAsync();

            if (resolvedInputOutput is not null)
            {
                bool hasUpdate = false;
                var address = resolvedInputOutput.Address;

                var lastLiquidityState = await _dbContext.LiquidityByAddress
                    .Where(lba => lba.Slot < slot && lba.Address == address)
                    .OrderByDescending(lba => lba.Slot)
                    .Take(1)
                    .FirstOrDefaultAsync();

                var assets = new Dictionary<string, Dictionary<string, ulong>>();

                if (lastLiquidityState is not null)
                {
                    assets = lastLiquidityState.Assets;
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

                if (assets.Count > 0 && hasUpdate)
                {
                    var thisBlockLiquidityState =
                        await _dbContext.LiquidityByAddress.Where(lba => lba.Slot == slot && lba.Address == address).FirstOrDefaultAsync() ??
                        _dbContext.LiquidityByAddress.Local.Where(lba => lba.Slot == slot && lba.Address == address).FirstOrDefault();

                    if (thisBlockLiquidityState is null)
                    {
                        thisBlockLiquidityState = new() { Address = address, Slot = slot, BlockNumber = blockNumber, Assets = assets };
                        _dbContext.LiquidityByAddress.Add(thisBlockLiquidityState);
                    }
                    else
                    {
                        thisBlockLiquidityState.Assets = assets;
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
            bool hasUpdate = false;
            var address = utxo.Address.ToBech32();

            var lastLiquidityState = await _dbContext.LiquidityByAddress
                .Where(lba => lba.Slot < slot && lba.Address == address)
                .OrderByDescending(lba => lba.Slot)
                .Take(1)
                .FirstOrDefaultAsync();

            var assets = new Dictionary<string, Dictionary<string, ulong>>();

            if (lastLiquidityState is not null)
            {
                assets = lastLiquidityState.Assets;
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
                        hasUpdate = true;

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

            if (assets.Count > 0 && hasUpdate)
            {
                var thisBlockLiquidityState =
                    await _dbContext.LiquidityByAddress.Where(lba => lba.Slot == slot && lba.Address == address).FirstOrDefaultAsync() ??
                    _dbContext.LiquidityByAddress.Local.Where(lba => lba.Slot == slot && lba.Address == address).FirstOrDefault();

                if (thisBlockLiquidityState is null)
                {
                    thisBlockLiquidityState = new() { Address = address, Slot = slot, BlockNumber = blockNumber, Assets = assets };
                    _dbContext.LiquidityByAddress.Add(thisBlockLiquidityState);
                }
                else
                {
                    thisBlockLiquidityState.Assets = assets;
                }

            }
        }

        await _dbContext.SaveChangesAsync();
    }

}