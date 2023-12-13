using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TeddySwap.Data.Models.Reducers;

public record LiquidityByAddressItem
{
    public string Address { get; init; } = default!;

    public ulong BlockNumber { get; init; }
    
    public ulong Slot { get; init; }

    public ulong Lovelace { get; set; }
    
    [NotMapped]
    public Dictionary<string, Dictionary<string, ulong>> Assets { get; set; } = default!;

    public JsonElement AssetsJson
    {
        get
        {
            var jsonString = JsonSerializer.Serialize(Assets);
            return JsonDocument.Parse(jsonString).RootElement;
        }

        set
        {
            if (value.ValueKind == JsonValueKind.Undefined || value.ValueKind == JsonValueKind.Null)
            {
                Assets = [];
            }
            else
            {
                Assets = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, ulong>>>(value.GetRawText()) ?? [];
            }
        }
    }
}