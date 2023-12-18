namespace TeddySwap.Data.Models;

public record YieldFarmingDistribution
{
    public ulong Slot { get; init; }
    public ulong BlockNumber { get; init; }
    public ulong Amount { get; init; }
    public ulong Bonus { get; init; }
}
