namespace TeddySwap.UI.Models;

public record Pool
{
    public TokenPair Pair { get; set; } = new();
    public decimal Fee { get; set; }
    public decimal Tvl { get; set; }
}
