using System.Text.Json;

namespace TeddySwap.Common.Models;

public class MintTransaction
{
    public string TxHash { get; init; } = string.Empty;
    public string PolicyId { get; init; } = string.Empty;
    public string TokenName { get; init; } = string.Empty;
    public string AsciiTokenName { get; init; } = string.Empty;
    public string? Metadata { get; set; }
    public string BlockHash { get; init; } = string.Empty;
}