using TeddySwap.Common.Enums;

namespace TeddySwap.Common.Models.Response;

public class OrderResponse
{
    public string Address { get; init; } = string.Empty;
    public OrderType OrderType { get; init; }
    public ulong Slot { get; init; }
    public ulong BlockNumber { get; init; }
    public string OrderX { get; init; } = string.Empty;
    public string OrderY { get; init; } = string.Empty;
    public string OrderLq { get; init; } = string.Empty;
}