namespace TeddySwap.Sink.Filters;

public class CardanoFilters
{
    public TransactionFilter? TransactionFilter { get; init; }
    public TxOutputFilter? TxOutputFilter { get; init; }
    public AssetFilter? AssetFilter { get; init; }
    public StakeDelegationFilter? StakeDelegationFilter { get; init; }
}