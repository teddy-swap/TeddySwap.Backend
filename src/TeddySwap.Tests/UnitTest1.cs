using TeddySwap.Data;
using TeddySwap.Data.Utils;
using TeddySwap.Sync.Reducers;

namespace TeddySwap.Tests;

public class UnitTest1
{
    [Fact]
    public void TotalDistributionAfterDurationIsCorrect()
    {
        var (_, _, CumulativeDistribution) = YieldFarmingUtils.GetMonthlyRewards(48);
        Assert.True(Math.Round(CumulativeDistribution) == 4320000);
    }
}