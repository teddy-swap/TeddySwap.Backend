using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TeddySwap.Data.Models.Reducers;

public class LedgerStateByAddress
{
    public string Address { get; set; } = default!;
    public ulong BlockNumber { get; set; }
    public ulong Slot { get; set; }

    [NotMapped]
    public List<TransactionOutput> Outputs { get; set; } = default!;

    public JsonElement OutputsJson
    {
        get
        {
            var jsonString = JsonSerializer.Serialize(Outputs);
            return JsonDocument.Parse(jsonString).RootElement;
        }

        set
        {
            if (value.ValueKind == JsonValueKind.Undefined || value.ValueKind == JsonValueKind.Null)
            {
                Outputs = [];
            }
            else
            {
                Outputs = JsonSerializer.Deserialize<List<TransactionOutput>>(value.GetRawText()) ?? [];
            }
        }
    }
}