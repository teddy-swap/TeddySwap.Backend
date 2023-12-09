namespace TeddySwap.Data.Models.Reducers;

public record class LovelaceByAddressItem (
    string Address,
    ulong Amount,
    ulong BlockNumber,
    ulong Slot
);