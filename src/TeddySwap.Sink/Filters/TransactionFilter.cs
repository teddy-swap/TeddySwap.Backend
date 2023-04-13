namespace TeddySwap.Sink.Filters;

public class TransactionFilter
{
    public List<string>? Addresses { get; init; }
    public List<string>? PolicyIds { get; init; }
}