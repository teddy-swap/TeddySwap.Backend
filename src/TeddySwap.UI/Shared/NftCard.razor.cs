using Microsoft.AspNetCore.Components;
using TeddySwap.Common.Models;
using TeddySwap.Common.Models.Response;
using TeddySwap.UI.Models;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Shared;

public partial class NftCard
{
    [Inject]
    protected NftService? NftService { get; set; }

    [Inject]
    protected RewardService? RewardService { get; set; }

    [Parameter, EditorRequired]
    public AssetResponse NftDetails { get; set; } = new AssetResponse();

    [Parameter, EditorRequired]
    public string PolicyId { get; set; } = string.Empty;

    public TbcNft? ExtraNftDetails
    {
        get
        {
            ArgumentNullException.ThrowIfNull(NftService);
            return NftService.GetNft(PolicyId, NftDetails.AsciiName);
        }
    }

    public NftRewardBreakdown RewardBreakdown
    {
        get
        {
            ArgumentNullException.ThrowIfNull(RewardService);
            return RewardService.CalculateNfTReward(PolicyId, NftDetails.AsciiName, NftDetails.MintOrder);
        }
    }
}
