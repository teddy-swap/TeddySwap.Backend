namespace TeddySwap.UI.Models;

public record Order
{
    public Token? TokenOne { get; set; }
    public Token? TokenTwo { get; set; }
    public decimal TokenOneAmount { get; set; }
    public decimal TokenTwoAmount { get; set; }
    public ActionType ActionType { get; set; }
    public DateTime DateTime { get; set; }
    public Status Status { get; set; }
}

public enum ActionType
{
    Swap,
    AddLiquidity,
    CreateLiquidity,
    RemoveLiquidity
}

public enum Status
{
    Complete,
    Canceled,
    Pending
}
