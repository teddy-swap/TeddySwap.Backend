using System.Numerics;
using TeddySwap.Common.Models.CardanoDbSync;

namespace TeddySwap.Common.Models.Explorer;

public class Output
{
    public string BlockHash { get; set; } = string.Empty;
    public string TxHash { get; set; } = string.Empty;
    public int Index { get; set; }
    public long GlobalIndex { get; set; }
    public string Address { get; set; } = string.Empty;
    public string RawAddr { get; set; } = string.Empty;
    public string? PaymentCred { get; set; }
    public string Lovelace { get; set; } = string.Empty;
    public List<OutputAsset> Value { get; set; } = new();
    public string? DataHash { get; set; }
    public Datum? Data { get; set; }
    public string? DataBin { get; set; }
    public string? RefScriptHash { get; set; }
}

