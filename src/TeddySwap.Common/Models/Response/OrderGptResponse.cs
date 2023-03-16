using TeddySwap.Common.Enums;

namespace TeddySwap.Common.Models.Response;

public class OrderGptResponse
{
    public string Prompt { get; init; } = string.Empty;
    public string Completion { get; init; } = string.Empty;
}