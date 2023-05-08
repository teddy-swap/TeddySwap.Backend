using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Utilities;
using Microsoft.Extensions.Options;
using TeddySwap.Sink.Models;

namespace TeddySwap.Sink.Services;

public class CardanoService
{
    private readonly TeddySwapSinkSettings _settings;
    public CardanoService(IOptions<TeddySwapSinkSettings> settings)
    {
        _settings = settings.Value;
    }

    public ulong CalculateEpochBySlot(ulong slot)
    {
        return _settings.NetworkType switch
        {
            NetworkType.Mainnet => (slot - 4492800) / 432000 + 208,
            _ => slot / _settings.EpochLength
        };
    }

    public string PoolHashToBech32(string poolId) => Bech32.Encode(poolId.HexToByteArray(), "pool");

    public string StakeHashToBech32(string stakeKeyHash)
    {
        byte[] byteArray = stakeKeyHash.HexToByteArray();
        if (byteArray.Count() > 28)
        {
            byteArray = byteArray.Skip(1).ToArray();
        }

        return AddressUtility.GetRewardAddress(byteArray, NetworkType.Preview).ToString();
    }

    public string? TryGetStakeAddress(string paymentAddress)
    {
        try
        {
            Address addr = new Address(paymentAddress);
            return addr.GetStakeAddress().ToString();
        }
        catch { }
        return null;
    }

    public string? GetStakeAddressFromEvent(OuraStakeDelegationEvent stakeDelegationEvent)
    {
        string? stakeKeyHash = string.IsNullOrEmpty(stakeDelegationEvent.StakeDelegation?.Credential?.AddrKeyHash) ?
                stakeDelegationEvent.StakeDelegation?.Credential?.Scripthash :
                stakeDelegationEvent.StakeDelegation.Credential.AddrKeyHash;

        return stakeKeyHash is not null ?
            AddressUtility.GetRewardAddress(Convert.FromHexString(stakeKeyHash), _settings.NetworkType).ToString() :
            null;
    }

    public bool IsInvalidTransaction(IEnumerable<ulong>? invalidTransactions, ulong transactionIndex) =>
        invalidTransactions is not null && invalidTransactions.ToList().Contains(transactionIndex);

}