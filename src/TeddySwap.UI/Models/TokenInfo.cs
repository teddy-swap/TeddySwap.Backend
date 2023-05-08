namespace TeddySwap.UI.Models;

public record TokenInfo
{
    public Token? Token { get; set; }
    public decimal AmountAdded { get; set; }
    public decimal CurrentBalance { get; set; }
}
