namespace TeddySwap.Data.Models.Reducers;
public record YieldRewardByAddress
{
    public string Address { get; set; }
    public string PoolId { get; set; }
    public ulong Amount { get; set; }
    public ulong LPAmount { get; set; }
    public ulong Bonus { get; set; }
    public decimal PoolShare { get; set; }
    public string[] TBCs { get; set; }
    public bool IsClaimed { get; set; }
    public ulong BlockNumber { get; set; }
    public ulong Slot { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string? ClaimTxId { get; set; }

    public YieldRewardByAddress(
        string address,
        string poolId,
        ulong amount,
        ulong lPAmount,
        ulong bonus,
        decimal poolShare,
        string[] tBCs,
        bool isClaimed,
        ulong blockNumber,
        ulong slot,
        DateTimeOffset timestamp,
        string? claimTxId = null)
    {
        Address = address;
        PoolId = poolId;
        Amount = amount;
        LPAmount = lPAmount;
        Bonus = bonus;
        PoolShare = poolShare;
        TBCs = tBCs;
        IsClaimed = isClaimed;
        BlockNumber = blockNumber;
        Slot = slot;
        Timestamp = timestamp;
        ClaimTxId = claimTxId;
    }
}
