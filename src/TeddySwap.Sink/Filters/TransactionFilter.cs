namespace TeddySwap.Sink.Filters;

public class TransactionFilter
{
    public List<string>? OutputAddresses { get; init; }
    public List<string>? MintPolicyIds { get; init; }
}