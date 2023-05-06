namespace TeddySwap.UI.Models;

public record Transaction
{
    public string CreatedTime { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Price { get; set; }
    public decimal Input { get; set; }
    public decimal Output { get; set; }
    public string Owner { get; set; } = string.Empty;
}

public enum TransactionType
{
    Buy,
    Sell
}
