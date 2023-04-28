namespace TeddySwap.Sink.Api.Models;

public record TeddySwapITNRewardSettings
{
    public int TotalReward { get; init; }
    public int BatcherReward { get; init; }
    public int UserReward { get; init; }
    public ulong ItnEndSlot { get; init; }
    public ulong FisoEndEpoch { get; init; }
    public decimal FisoRewardPerEpoch { get; init; }
    public string TbcPolicyId { get; init; } = string.Empty;
}