namespace TeddySwap.Data.Models.Reducers;
public record YieldRewardByAddress (
    string Address,
    string PoolId,
    ulong Amount,
    ulong LPAmount,
    decimal PoolShare,
    bool IsClaimed,
    string? ClaimTxId,
    ulong BlockNumber,
    ulong Slot,
    DateTimeOffset Timestamp
);