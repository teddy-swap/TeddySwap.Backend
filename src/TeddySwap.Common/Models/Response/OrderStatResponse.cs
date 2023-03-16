using TeddySwap.Common.Enums;

namespace TeddySwap.Common.Models.Response;

public class OrderStatResponse
{
    public decimal AvgInterval { get; init; }
    public decimal StdInterval { get; init; }
    public decimal AvgPerDay { get; init; }
    public decimal StdPerDay { get; init; }
    public decimal AvgPerHour { get; init; }
    public decimal StdPerHour { get; init; }
}