namespace TeddySwap.UI.Models;

public record TokenPair
{
    public (Token Token1, Token Token2) Tokens { get; set; } = new();
}
