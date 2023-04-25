namespace TeddySwap.Sink.Filters;

public record CardanoFilters
{
    public TransactionFilter? TransactionFilter { get; set; }
    public TxOutputFilter? TxOutputFilter { get; set; }
    public AssetFilter? AssetFilter { get; set; }
    public StakeDelegationFilter? StakeDelegationFilter { get; set; }
}