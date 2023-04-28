using System.Text.Json.Serialization;

namespace TeddySwap.Sink.Models.Oura;

public class OuraAsset
{
    public string? Policy { get; init; }
    public string? Asset { get; init; }
    [JsonPropertyName("asset_ascii")]
    public string? AssetAscii { get; init; }
    public ulong? Amount { get; init; }
}