namespace TeddySwap.Data.Models.Reducers;

public record YieldClaimRequest
{
    public string Address { get; set; }
    public ulong BlockNumber { get; set; }
    public ulong Slot { get; set; }
    public string TxHash { get; set; }
    public ulong TxIndex { get; set; }
    public string[] TBCs { get; set; }
    public string? ProcessTxHash { get; set; }
    public ulong? ProcessBlockNumber { get; set; }
    public ulong? ProcessSlot { get; set; }

    public YieldClaimRequest(
        string address,
        ulong blockNumber,
        ulong slot,
        string txHash,
        ulong txIndex,
        string[] tBCs,
        string? processTxHash,
        ulong? processBlockNumber,
        ulong? processSlot)
    {
        Address = address;
        BlockNumber = blockNumber;
        Slot = slot;
        TxHash = txHash;
        TxIndex = txIndex;
        TBCs = tBCs;
        ProcessTxHash = processTxHash;
        ProcessBlockNumber = processBlockNumber;
        ProcessSlot = processSlot;
    }
}
