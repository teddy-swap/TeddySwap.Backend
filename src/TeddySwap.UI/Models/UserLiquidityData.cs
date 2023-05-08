namespace TeddySwap.UI.Models;

public record UserLiquidityData
{
    public TokenInfo? TokenOneInfo { get; set; }
    public TokenInfo? TokenTwoInfo { get; set; }
    public double FeeShare { get; set; }
    public decimal ImpermanentLoss { get; set; }
    public int Number { get; set; }
    public bool ShowDetails { get; set; }
}
