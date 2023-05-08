using System.Text.Json;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Addresses;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.Common.Models;
using TeddySwap.Common.Models.Response;
using TeddySwap.Common.Services;
using TeddySwap.Common.Utils;
using TeddySwap.UI.Models;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Pages;

public partial class Rewards : IAsyncDisposable
{
    private const string ROUND_ONE_POLICY_ID = "ab182ed76b669b49ee54a37dee0d0064ad4208a859cc4fdf3f906d87";
    private const string ROUND_TWO_POLICY_ID = "da3562fad43b7759f679970fb4e0ec07ab5bebe5c703043acda07a3c";

    [Inject]
    protected CardanoWalletService? CardanoWalletService { get; set; }

    [Inject]
    protected SinkService? SinkService { get; set; }

    [Inject]
    protected QueryService? QueryService { get; set; }

    [Inject]
    protected NftService? NftService { get; set; }

    [Inject]
    protected ISnackbar? Snackbar { get; set; }

    [Inject]
    protected RewardService? RewardService { get; set; }

    [Inject]
    protected ILogger<Rewards>? Logger { get; set; }

    protected LeaderBoardResponse LeaderBoardResponse { get; set; } = new LeaderBoardResponse();

    protected decimal TotalRewards
    {
        get
        {
            ArgumentNullException.ThrowIfNull(RewardService);

            decimal roundOneSum = 0;
            decimal roundTwoSum = 0;

            RoundOneAssets?.Result.ToList().ForEach(a =>
            {
                NftRewardBreakdown nrb = RewardService.CalculateNfTReward(ROUND_ONE_POLICY_ID, a.AsciiName, a.MintOrder);
                roundOneSum += nrb.TotalReward;
            });


            RoundTwoAssets?.Result.ToList().ForEach(a =>
            {
                NftRewardBreakdown nrb = RewardService.CalculateNfTReward(ROUND_TWO_POLICY_ID, a.AsciiName, a.MintOrder);
                roundTwoSum += nrb.TotalReward;
            });
            return LeaderBoardResponse.BaseReward + TotalItnNftBonus + TotalFisoRewards + roundOneSum + roundTwoSum;
        }
    }

    protected bool IsTestnetRewardsLoaded { get; set; }
    protected bool IsClaimDialogShown { get; set; }
    protected bool IsMainnet { get; set; }
    protected string MainnetAddress { get; set; } = string.Empty;
    protected int TotalRoundOneNft { get; set; }
    protected int TotalRoundTwoNft { get; set; }
    protected decimal TotalRoundOneItnNftBonus { get; set; }
    protected decimal TotalRoundTwoItnNftBonus { get; set; }
    protected decimal TotalItnNftBonus { get; set; }
    protected decimal BaseFisoRewards { get; set; }
    protected decimal TotalFisoRewards { get; set; }
    protected PaginatedAssetResponse? RoundOneAssets { get; set; }
    protected PaginatedAssetResponse? RoundTwoAssets { get; set; }
    protected int RoundOnePage { get; set; } = 1;
    protected int RoundTwoPage { get; set; } = 1;

    protected List<NftDetails> _nfts1 { get; set; } = default!;

    protected List<NftDetails> _nfts2 { get; set; } = default!;

    private double? totalBonus { get; set; }

    private int? bonus { get; set; }

    public int? baseReward { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ArgumentNullException.ThrowIfNull(CardanoWalletService);
        ArgumentNullException.ThrowIfNull(NftService);
        CardanoWalletService.ConnectionStateChange += OnConnectionStateChanged;
        await NftService.InitializeNftDataAsync();

        await RefreshDataAsync();
        await base.OnInitializedAsync();
    }

    protected override async void OnHeartBeatEvent(object? sender, EventArgs e)
    {
        await RefreshDataAsync();
        base.OnHeartBeatEvent(sender, e);
    }

    private async void OnConnectionStateChanged(object? sender, EventArgs e)
    {
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        IsTestnetRewardsLoaded = false;
        await InvokeAsync(StateHasChanged);

        ArgumentNullException.ThrowIfNull(SinkService);
        ArgumentNullException.ThrowIfNull(CardanoWalletService);
        ArgumentNullException.ThrowIfNull(QueryService);
        ArgumentNullException.ThrowIfNull(HeartBeatService);

        if (!string.IsNullOrEmpty(CardanoWalletService.ConnectedAddress))
        {
            try
            {
                Address connectedAddress = new Address(CardanoWalletService.ConnectedAddress);
                IsMainnet = connectedAddress.NetworkType == CardanoSharp.Wallet.Enums.NetworkType.Mainnet;

                if (!IsMainnet)
                {
                    string[] addresses = await QueryService.Query($"CardanoWalletService.GetUsedAddressesAsync:{CardanoWalletService.SessionId}:{HeartBeatService.LatestSlotNo}", async () =>
                    {
                        return await CardanoWalletService.GetUsedAddressesAsync();
                    });

                    addresses = addresses.Length <= 0 ? new string[] { CardanoWalletService.ConnectedAddress } : addresses;

                    PaginatedLeaderBoardResponse response = await QueryService.Query($"/leaderboard/users/addresses/${string.Join(",", addresses)}", async () =>
                    {
                        return await SinkService.GetRewardFromAddressesAsync(addresses);
                    });

                    LeaderBoardResponse = response.Result.FirstOrDefault() ?? new LeaderBoardResponse();

                    MainnetAddress = await QueryService.Query($"SinkService.GetMainnetAddressFromTestnetAddressAsync:{CardanoWalletService.ConnectedAddress}:{HeartBeatService.LatestSlotNo}:{MainnetAddress}", async () =>
                    {
                        return await SinkService.GetMainnetAddressFromTestnetAddressAsync(new Address(CardanoWalletService.ConnectedAddress).GetStakeAddress().ToString());
                    });
                }
                else
                {
                    LeaderBoardResponse = new LeaderBoardResponse();
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error Retrieving Wallet Addresses!");
            }

            try
            {
                string rawQueryAddress = IsMainnet ? CardanoWalletService.ConnectedAddress : MainnetAddress;
                if (!string.IsNullOrEmpty(rawQueryAddress))
                {
                    string queryAddress = new Address(rawQueryAddress).GetStakeAddress().ToString();

                    TotalRoundOneNft = await QueryService.Query($"SinkService.GetNftCountByAddressPolicy:{CardanoWalletService.SessionId}:{queryAddress}:ab182ed76b669b49ee54a37dee0d0064ad4208a859cc4fdf3f906d87:{HeartBeatService.LatestSlotNo}", async () =>
                    {
                        return await SinkService.GetNftCountByStakeAddressPolicyAsync(queryAddress, ROUND_ONE_POLICY_ID);
                    });

                    TotalRoundTwoNft = await QueryService.Query($"SinkService.GetNftCountByAddressPolicy:{CardanoWalletService.SessionId}:{queryAddress}:da3562fad43b7759f679970fb4e0ec07ab5bebe5c703043acda07a3c:{HeartBeatService.LatestSlotNo}", async () =>
                    {
                        return await SinkService.GetNftCountByStakeAddressPolicyAsync(queryAddress, ROUND_TWO_POLICY_ID);
                    });

                    TotalRoundOneItnNftBonus = TotalRoundOneNft * 5;
                    TotalRoundTwoItnNftBonus = TotalRoundTwoNft * 2;
                    TotalItnNftBonus = (TotalRoundOneItnNftBonus + TotalRoundTwoItnNftBonus) / 100 * LeaderBoardResponse.BaseReward;
                }
            }
            catch (Exception ex)
            {
                TotalRoundOneNft = 0;
                TotalRoundTwoNft = 0;
                TotalRoundOneItnNftBonus = 0;
                TotalRoundTwoItnNftBonus = 0;
                TotalItnNftBonus = 0;
                Logger?.LogError(ex, "Error Calculating Nft Rewards!");
            }

            try
            {
                string queryAddress = IsMainnet ? CardanoWalletService.ConnectedAddress : MainnetAddress;
                string mainnetStakeAddress = new Address(queryAddress).GetStakeAddress().ToString();
                BaseFisoRewards = await QueryService.Query($"SinkService.GetFisoRewardByStakeAddressAsync:{CardanoWalletService.SessionId}:{mainnetStakeAddress}:{HeartBeatService.LatestSlotNo}", async () =>
                {
                    return (decimal)await SinkService.GetFisoRewardByStakeAddressAsync(mainnetStakeAddress);
                });

                TotalFisoRewards = BaseFisoRewards + (BaseFisoRewards * TotalRoundOneNft * 0.05M) + (BaseFisoRewards * TotalRoundTwoNft * 0.02M);
            }
            catch (Exception ex)
            {
                BaseFisoRewards = 0;
                TotalFisoRewards = 0;
                Logger?.LogError(ex, "Error Calculating FISO Rewards!");
            }

            try
            {
                string queryAddress = new Address(IsMainnet ? CardanoWalletService.ConnectedAddress : MainnetAddress).GetStakeAddress().ToString();

                RoundOneAssets =
                    await SinkService.GetPolicyAssetsByStakeAddressAsync(
                        queryAddress,
                        ROUND_ONE_POLICY_ID,
                        10_000,
                        0
                    );

                RoundTwoAssets =
                    await SinkService.GetPolicyAssetsByStakeAddressAsync(
                        queryAddress,
                        ROUND_TWO_POLICY_ID,
                        10_000,
                        0
                    );

            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error Retrieveing NFT Assets!");
            }
        }

        IsTestnetRewardsLoaded = true;

        await InvokeAsync(StateHasChanged);
    }

    public async void OnClaimClicked()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(CardanoWalletService);
            ArgumentNullException.ThrowIfNull(QueryService);
            IsClaimDialogShown = true;
            await InvokeAsync(StateHasChanged);

        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error Showing Claim Dialog!");
        }
    }

    public async void OnClaimSubmit()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(CardanoWalletService);
            ArgumentNullException.ThrowIfNull(SinkService);
            ArgumentNullException.ThrowIfNull(QueryService);
            ArgumentNullException.ThrowIfNull(Snackbar);

            string rewardAddress = await QueryService.Query($"CardanoWalletService.GetStakeAddressAsync:{CardanoWalletService.SessionId}", async () =>
            {
                return await CardanoWalletService.GetStakeAddressAsync();
            });
            string[] addresses = new string[] { rewardAddress };

            string newMainnetAddress = MainnetAddress;

            string messageJson = JsonSerializer.Serialize(new LinkAddressPayload
            {
                MainnetAddress = newMainnetAddress,
                TestnetAddresses = addresses
            });

            CardanoSignedMessage signedMessage = await CardanoWalletService.SignMessage(messageJson.ToHex());
            await SinkService.LinkMainnetAddressAsync(rewardAddress, messageJson.ToHex(), signedMessage);

            await RefreshDataAsync();
            IsClaimDialogShown = false;
            await InvokeAsync(StateHasChanged);

            MainnetAddress = newMainnetAddress;
            Snackbar.Add("You have succesfully linked your mainnet address! 🎊", Severity.Success);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error Linking Mainnet Address!");
        }
    }

    protected void OnRoundOnePaginationChanged(int page)
    {
        RoundOnePage = page;
        StateHasChanged();
    }

    new public async ValueTask DisposeAsync()
    {
        ArgumentNullException.ThrowIfNull(CardanoWalletService);
        CardanoWalletService.ConnectionStateChange -= OnConnectionStateChanged;
        await base.DisposeAsync();
    }
}
