namespace TeddySwap.Data.Utils;

public static class YieldFarmingUtils
{
    public const ulong YF_START_REWARD_AMOUNT = 178217820000;
    public const ulong YF_MONTHLY_DECREASE = 3753950000;
    public const int YF_TOTAL_MONTHS = 48;
    public const ulong YF_SECONDS_IN_MONTH = 2592000;
    public const long YF_SECONDS_IN_DAY = 86400;
    public const ulong YF_START_SLOT = 109812631; // 1701378922 unix timestamp, 12/01/2023 5:15:22 AM UTC
    public const long YF_START_TIME = 1701378922;
    public const ulong MAX_LP_TOKENS = 9223372036854775807;

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

    public static decimal GetDailyRewardAmount(int month)
    {
        var (_, MonthlyDistribution, _) = GetMonthlyRewards(month);
        return MonthlyDistribution / 30;
    }

    public static (int Month, decimal MonthlyDistribution, decimal CumulativeDistribution) GetMonthlyRewards(int month)
    {
        if (month < 1 || month > YieldFarmingUtils.YF_TOTAL_MONTHS)
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

}