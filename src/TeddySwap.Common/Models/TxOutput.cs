namespace TeddySwap.Common.Models;

public partial record TxOutputBase
{
    public string TxHash { get; init; } = string.Empty;
    public ulong TxIndex { get; init; }
    public ulong Index { get; init; }
    public ulong Amount { get; init; }
    public string Address { get; init; } = string.Empty;
    public string? DatumCbor { get; init; }
    public string Blockhash { get; init; } = string.Empty;
}