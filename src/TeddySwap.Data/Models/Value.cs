using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TeddySwap.Data.Models;

public record Value
{
    public ulong Coin { get; init; } = default!;

    [NotMapped]
    public Dictionary<string, Dictionary<string, ulong>> MultiAsset { get; set; } = default!;

    public string MultiAssetJson
    {
        get => JsonSerializer.Serialize(MultiAsset);
        set => MultiAsset = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, ulong>>>(value) ?? [];
    }
}