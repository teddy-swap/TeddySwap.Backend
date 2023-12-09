namespace TeddySwap.Data;

public record class Block (
    string Id,
    ulong Number,
    ulong Slot
);
