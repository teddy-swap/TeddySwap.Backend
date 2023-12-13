using TeddySwap.Data;
using TeddySwap.Sync.Reducers;

namespace TeddySwap.Tests;

public class UnitTest1
{
    [Fact]
    public void TotalDistributionAfterDurationIsCorrect()
    {
        var (_, _, CumulativeDistribution) = TeddyYieldFarmingReducer.GetMonthlyRewards(48);
        Assert.True(Math.Round(CumulativeDistribution) == 4320000);
    }
}