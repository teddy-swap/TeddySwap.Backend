namespace TeddySwap.UI.Models;

public record Pool
{
    public TokenPair Pair { get; set; } = new();
    public double Fee { get; set; }
    public decimal Tvl { get; set; }
}
