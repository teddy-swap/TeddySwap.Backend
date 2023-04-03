namespace TeddySwap.Common.Models;

public partial record CollateralTxOut
{
    public string TxHash { get; init; } = string.Empty;
    public ulong TxIndex { get; init; }
    public Transaction Transaction { get; init; } = new();
    public ulong Index { get; init; }
    public ulong Amount { get; init; }
    public string Address { get; init; } = string.Empty;
}