
using TeddySwap.Common.Enums;

namespace TeddySwap.Common.Models.Response;

public class AddressOrderStatResponse
{
    public string Address { get; init; } = string.Empty;
    public decimal AvgInterval { get; init; }
    public decimal AvgPerDay { get; init; }
    public decimal AvgPerHour { get; init; }
    public decimal BotScore { get; set; }
}