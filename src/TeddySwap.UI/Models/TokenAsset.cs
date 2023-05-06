namespace TeddySwap.UI.Models;

public record TokenAsset
{
    public Token? Token { get; set; }
    public decimal Amount { get; set; }
}
