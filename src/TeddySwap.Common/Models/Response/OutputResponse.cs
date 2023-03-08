
using System.Numerics;
using System.Text.Json;
using TeddySwap.Common.Models.Explorer;

namespace TeddySwap.Common.Models.Response;

public class OutputResponse
{
    public Output Output { get; init; } = new();
}