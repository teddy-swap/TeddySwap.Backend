using System.Numerics;

namespace TeddySwap.Common.Models.Response;

public record LiquidityPoolResponse
{
    public string NftUnit { get; init; } = string.Empty;
    public string LqUnit { get; init; } = string.Empty;
    public string AssetXUnit { get; init; } = string.Empty;
    public string AssetYUnit { get; init; } = string.Empty;
    public decimal Fee { get; init; }
    public string ReservesX { get; init; } = string.Empty;
    public string ReservesY { get; init; } = string.Empty;
}