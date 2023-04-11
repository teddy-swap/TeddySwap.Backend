using System.Text.Json;
using System.Text.Json.Serialization;

namespace TeddySwap.Sink.Models.Oura;

public record OuraAssetEvent : OuraEvent
{
    public string Address { get; set; } = string.Empty;
    public string PolicyId { get; init; } = string.Empty;
    public string TokenName { get; init; } = string.Empty;
    public ulong Amount { get; init; }
    public string? TxHash { get; set; } = string.Empty;
    public ulong? OutputIndex { get; set; }
}