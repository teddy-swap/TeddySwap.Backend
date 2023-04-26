using System.Text.Json;

namespace TeddySwap.Common.Models.Response;

public record VerifyMessageResponse
{
    public bool HasSigned { get; init; }
}