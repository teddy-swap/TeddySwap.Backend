using System.Numerics;

namespace TeddySwap.Common.Models;

public record Price
{
    public string TxHash { get; init; } = string.Empty;
    public ulong Index { get; init; }
    public decimal PriceX { get; init; }
    public decimal PriceY { get; init; }
    public string AssetX { get; init; } = string.Empty;
    public string AssetY { get; init; } = string.Empty;
    public string AssetLq { get; init; } = string.Empty;
    public string PoolNft { get; init; } = string.Empty;
    public ulong Slot { get; init; }
    public string Blockhash { get; set; } = string.Empty;
}