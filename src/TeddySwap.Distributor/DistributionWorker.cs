
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Models.Transactions;
using CardanoSharp.Wallet.TransactionBuilding;
using CardanoSharp.Wallet.Utilities;
using TeddySwap.Data.Models;
using TeddySwap.Data.Models.Reducers;
using TeddySwap.Data.Services;

namespace TeddySwap.Distributor;

public class DistributionWorker(
    BlockDataService blockDataService,
    LedgerStateDataService ledgerStateDataService,
    YieldFarmingDataService yieldFarmingDataService,
    TransactionDataService transactionDataService,
    MnemonicService mnemonicService,
    ILogger<DistributionWorker> logger,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory
) : BackgroundService
{
    private ulong LastProcessedBlock { get; set; } = 0;
    private PrivateKey PaymentPrivateKey { get; set; } = default!;
    private PrivateKey StakePrivateKey { get; set; } = default!;
    private PublicKey PaymentPublicKey { get; set; } = default!;
    private PublicKey StakePublicKey { get; set; } = default!;
    private Address DistributionAddress { get; set; } = default!;
    private readonly string[] _tbcPolicies = [
        "ab182ed76b669b49ee54a37dee0d0064ad4208a859cc4fdf3f906d87",
        "da3562fad43b7759f679970fb4e0ec07ab5bebe5c703043acda07a3c",
    ];
    private readonly string _TedyTokenUnit = "f6696363e9196289ef4f2b4bf34bc8acca5352cdc7509647afe6888f.54454459";
    private readonly decimal tbcOneBonus = 0.01m; // 1% bonus
    private readonly decimal tbcTwoBonus = 0.004m; // 2% bonus

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mnemonic = mnemonicService.Restore(configuration["TeddySwapDistributorSeed"]!);

        ExtractKeysFromMnemonic(mnemonic, 0);

        while (!stoppingToken.IsCancellationRequested)
        {
            var currentBlock = await blockDataService.GetLatestBlockAsync();

            if (currentBlock is null)
            {
                logger.LogError("Failed to get latest block");
                await Task.Delay(1000 * 20, stoppingToken);
                continue;
            }

            if (currentBlock.Number <= LastProcessedBlock)
            {
                await Task.Delay(1000 * 20, stoppingToken);
                continue;
            }

            await ProcessClaimRequestsAsync(currentBlock);

            await Task.Delay(1000 * 20, stoppingToken);
        }
    }

    private async Task ProcessClaimRequestsAsync(Block currentBlock)
    {
        var ledgerState = await ledgerStateDataService.LedgerStateByAddressAsync(DistributionAddress.ToString());
        var pendingClaimRequests = await yieldFarmingDataService.GetPendingYieldClaimRequestsAsync();
        var claimRequests = pendingClaimRequests.Where(cr => currentBlock.Number - cr.BlockNumber >= 10);

        logger.LogInformation(
            "[{blockNumber}]: Total ClaimRequests [{pendingClaims}], Confirmed [{confirmedClaims}]", 
            currentBlock.Number, pendingClaimRequests.Count(), claimRequests.Count()
        );

        if (claimRequests is not null && claimRequests.Any())
        {
            if (ledgerState is not null && ledgerState.Outputs.Count > 0)
            {
                var recentTbcs = await yieldFarmingDataService.GetClaimedTbcLastDayAsync(currentBlock.Slot);
                var transactionBody = TransactionBodyBuilder.Create;
                var totalLovelace = (ulong)ledgerState.Outputs.Sum(o => (decimal)o.Amount.Coin);
                var assets = ledgerState.Outputs.SelectMany(o => o.Amount.MultiAsset).ToList();
                var consumedAssets = new List<string>();
                var tedyUnitSplit = _TedyTokenUnit.Split(".");

                var tedyAvailable = (ulong)assets
                    .Where(a => a.Key == tedyUnitSplit[0] && a.Value.ContainsKey(tedyUnitSplit[1]))
                    .Select(a => a.Value[tedyUnitSplit[1]])
                    .Sum(a => (decimal)a);

                assets.RemoveAll(a => a.Key == tedyUnitSplit[0]);

                var processedClaimRequests = new List<YieldClaimRequest>();
                var processedYieldRewards = new List<YieldRewardByAddress>();
                var claimRequestsAsQueue = new Queue<YieldClaimRequest>(claimRequests.OrderBy(r => r.Slot));

                // Add inputs
                foreach (var output in ledgerState.Outputs)
                {
                    transactionBody.AddInput(output.Id.HexToByteArray(), output.Index);
                }

                while (claimRequestsAsQueue.TryDequeue(out var claimRequest))
                {
                    var claimRequestAddress = claimRequest.Address;
                    var yieldRewardsByAddress = await yieldFarmingDataService.UnclaimedYieldRewardByAddressAsync(claimRequestAddress);
                    var totalReward = (ulong)yieldRewardsByAddress.Sum(r => (decimal)r.Amount);
                    var returnAda = claimRequest.TBCs.Length != 0 ? 4_200_000ul : 1_700_000ul;

                    var totalBonusPercent =
                        claimRequest.TBCs.Where(tbc => tbc.StartsWith(_tbcPolicies[0]) && !recentTbcs.Contains(tbc)).Count() * tbcOneBonus +
                        claimRequest.TBCs.Where(tbc => tbc.StartsWith(_tbcPolicies[1]) && !recentTbcs.Contains(tbc)).Count() * tbcTwoBonus;

                    var totalBonus = (ulong)(totalReward * totalBonusPercent);
                    var totalRewardPlusBonus = totalReward + totalBonus;
                    yieldRewardsByAddress.ToList().ForEach(r => {
                        r.Bonus = (ulong)(r.Amount * totalBonusPercent);
                        r.TBCs = claimRequest.TBCs;
                    });
                    processedYieldRewards.AddRange(yieldRewardsByAddress);

                    var rewardAssets = new Dictionary<byte[], NativeAsset>() { };

                    if (totalRewardPlusBonus > 0)
                    {
                        rewardAssets.Add(
                            Convert.FromHexString(tedyUnitSplit[0]),
                            new()
                            {
                                Token = new Dictionary<byte[], long>
                                {
                                    { Convert.FromHexString(tedyUnitSplit[1]), (long)totalRewardPlusBonus }
                                }
                            }
                        );
                    }

                    if (claimRequest.TBCs.Length > 0)
                    {
                        var consumedTbcOneAssets = assets
                            .Where(a => a.Key == _tbcPolicies[0])
                            .SelectMany(a => a.Value)
                            .Where(a => claimRequest.TBCs.Contains(_tbcPolicies[0] + "." + a.Key))
                            .ToDictionary(a => Convert.FromHexString(a.Key), a => 1L);

                        var consumedTbcTwoAssets = assets
                            .Where(a => a.Key == _tbcPolicies[1])
                            .SelectMany(a => a.Value)
                            .Where(a => claimRequest.TBCs.Contains(_tbcPolicies[1] + "." + a.Key))
                            .ToDictionary(a => Convert.FromHexString(a.Key), a => 1L);

                        rewardAssets.Add(Convert.FromHexString(_tbcPolicies[0]),
                            new() { Token = consumedTbcOneAssets }
                        );

                        rewardAssets.Add(Convert.FromHexString(_tbcPolicies[1]),
                            new() { Token = consumedTbcTwoAssets }
                        );

                        consumedAssets.AddRange(claimRequest.TBCs);
                    }

                    transactionBody.AddOutput(new()
                    {
                        Address = new Address(claimRequestAddress).GetBytes(),
                        Value = new()
                        {
                            Coin = returnAda,
                            MultiAsset = rewardAssets.Count > 0 ? rewardAssets : []
                        }
                    });

                    totalLovelace -= returnAda;
                    tedyAvailable -= totalRewardPlusBonus;

                    processedClaimRequests.Add(claimRequest);

                    var tempTxBody = transactionBody.Build();
                    var tempWitness = TransactionWitnessSetBuilder.Create
                    .AddVKeyWitness(PaymentPublicKey, PaymentPrivateKey).Build();

                    var tempTx = new Transaction()
                    {
                        TransactionBody = tempTxBody,
                        TransactionWitnessSet = tempWitness
                    };

                    // if greater than 10kb then proceed to submit
                    if (tempTx.Serialize().Length > 10_000)
                        break;
                }

                // Add change output

                var fee = claimRequests.Where(cr => cr.TBCs.Length != 0).Any() ? 800_000ul : 300_000ul;
                totalLovelace -= fee;

                var changeAssets = assets
                    .Where(a =>
                    {
                        return !consumedAssets.Any(
                            ca =>
                            {
                                var caSplit = ca.Split(".");
                                return a.Key == caSplit[0] && a.Value.ContainsKey(caSplit[1]);
                            }
                        );
                    })
                    .ToDictionary(a => Convert.FromHexString(a.Key), a => a.Value);

                var changeMultiAsset = new Dictionary<byte[], NativeAsset>();

                if (tedyAvailable > 0)
                {
                    changeMultiAsset.Add(Convert.FromHexString(tedyUnitSplit[0]),
                        new()
                        {
                            Token = new Dictionary<byte[], long>
                            {
                                { Convert.FromHexString(tedyUnitSplit[1]), (long)tedyAvailable }
                            }
                        }
                    );
                }

                if (changeAssets.Count > 0)
                {
                    foreach (var asset in changeAssets)
                    {
                        asset.Value
                            .ToList()
                            .ForEach(
                                a => changeMultiAsset.Add(asset.Key, new() { Token = new() { { Convert.FromHexString(a.Key), (long)a.Value } } })
                            );
                    }
                }

                transactionBody.AddOutput(new()
                {
                    Address = DistributionAddress.GetBytes(),
                    Value = new()
                    {
                        Coin = totalLovelace,
                        MultiAsset = changeMultiAsset
                    }
                });

                // 1 hour
                transactionBody.SetTtl((uint)currentBlock.Slot + 60 * 60);
                transactionBody.SetFee(fee);
                var txBody = transactionBody.Build();
                var witnesses = TransactionWitnessSetBuilder.Create
                    .AddVKeyWitness(PaymentPublicKey, PaymentPrivateKey).Build();

                var tx = new Transaction()
                {
                    TransactionBody = txBody,
                    TransactionWitnessSet = witnesses
                };

                var txHash = await SubmitTxAsync(tx.Serialize());

                if (!string.IsNullOrEmpty(txHash))
                {
                    logger.LogInformation("Successfully submitted transaction {TxHash}", txHash);
                    await yieldFarmingDataService.SetYieldRewardByAddressClaimedAsync(processedYieldRewards, txHash);
                    await yieldFarmingDataService.SetYieldClaimRequestsProcessedAsync(processedClaimRequests, txHash, currentBlock.Number, currentBlock.Slot);
                    if (await WaitTxConfirmationsWithTimeoutAsync(txHash, 1, 250))
                    {
                        logger.LogInformation("Successfully confirmed transaction {TxHash}", txHash);
                    }
                    else
                    {
                        logger.LogError("Failed to submit transaction {TxHash}, possibly rolledback!", txHash);
                        return;
                    }
                }
                else
                {
                    logger.LogError("Failed to submit transaction");
                }
            }
        }
    }

    private async Task<bool> WaitTxConfirmationsWithTimeoutAsync(string txHash, ulong confirmationsNeeded, int timeoutSeconds)
    {
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow.Subtract(start).TotalSeconds < timeoutSeconds)
        {
            ulong confirmations = await transactionDataService.GetTransactionIdConfirmationsAsync(txHash);

            if (confirmations >= confirmationsNeeded)
            {
                return true;
            }

            await Task.Delay(1000 * 5);
        }

        return false;
    }

    private async Task<string?> SubmitTxAsync(byte[] txBytes)
    {
        using var httpClient = httpClientFactory.CreateClient();
        var content = new ByteArrayContent(txBytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/cbor");
        content.Headers.Add("project_id", configuration["BlockfrostApiKey"]);
        var response = await httpClient.PostAsync(configuration["TxSubmitEndpoint"], content);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<string>();
        }
        else
        {
            return string.Empty;
        }
    }

    private void ExtractKeysFromMnemonic(Mnemonic mnemonic, int index)
    {
        var rootKey = mnemonic.GetRootKey();
        string paymentPath = $"m/1852'/1815'/0'/0/{index}";
        PrivateKey paymentPrv = rootKey.Derive(paymentPath);
        PublicKey paymentPub = paymentPrv.GetPublicKey(false);

        string stakePath = $"m/1852'/1815'/0'/2/{index}";
        PrivateKey stakePrv = rootKey.Derive(stakePath);
        PublicKey stakePub = stakePrv.GetPublicKey(false);

        var address = AddressUtility.GetBaseAddress(
            paymentPub,
            stakePub,
            NetworkType.Mainnet);

        PaymentPrivateKey = paymentPrv;
        PaymentPublicKey = paymentPub;
        StakePrivateKey = stakePrv;
        StakePublicKey = stakePub;

        DistributionAddress = address;
        logger.LogInformation("Distributor Address: {address}", address);
    }
}