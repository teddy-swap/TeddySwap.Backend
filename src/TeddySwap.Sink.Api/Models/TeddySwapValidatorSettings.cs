
using CardanoSharp.Wallet.Enums;

public record TeddySwapValidatorSettings
{
    public string PoolAddress { get; init; } = string.Empty;
    public string DepositAddress { get; init; } = string.Empty;
    public string RedeemAddress { get; init; } = string.Empty;
    public string SwapAddress { get; init; } = string.Empty;
    public NetworkType NetworkType { get; init; }
}