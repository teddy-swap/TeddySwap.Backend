using System.Numerics;
using TeddySwap.Common.Enums;

namespace TeddySwap.Common.Models.Response;

public record SwapOrderResponse
{
    public string TxHash { get; init; } = string.Empty;
    public ulong OutputIndex { get; init; }
    public string BaseAsset { get; init; } = string.Empty;
    public string BaseAmount { get; init; } = string.Empty;
    public string QuoteAsset { get; init; } = string.Empty;
    public string? QuoteAmount { get; init; }
    public string Owner { get; init; } = string.Empty;
    public ulong Slot { get; init; }
    public OrderStatus OrderStatus { get; init; }
}