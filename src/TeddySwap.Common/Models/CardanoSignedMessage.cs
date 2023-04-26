namespace TeddySwap.Common.Models;

public record CardanoSignedMessage
{
    public string? Key { get; init; }
    public string? Signature { get; init; }
}