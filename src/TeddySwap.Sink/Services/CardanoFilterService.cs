using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Utilities;
using Microsoft.Extensions.Options;
using TeddySwap.Sink.Filters;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Services;

public class CardanoFilterService
{
    private readonly CardanoFilters _cardanoFilters;
    public CardanoFilterService(IOptions<CardanoFilters> cardanoFilters)
    {
        _cardanoFilters = cardanoFilters.Value;
    }

    public List<OuraTransaction> FilterTransactions(List<OuraTransaction>? ouraTransactions)
    {
        if (_cardanoFilters.TransactionFilter is not null && ouraTransactions is not null)
        {
            List<string>? addresses = _cardanoFilters.TransactionFilter.OutputAddresses ?? null;
            List<string>? mintPolicyIds = _cardanoFilters.TransactionFilter.MintPolicyIds ?? null;

            ouraTransactions = ouraTransactions
                .Where(t =>
                {
                    if (addresses is not null || mintPolicyIds is not null)
                    {
                        bool hasAddressMatch = false;
                        bool hasMintPolicyIdMatch = false;

                        if (addresses is not null && t.Outputs is not null)
                            hasAddressMatch = t.Outputs.Any(o => addresses.Contains(o.Address!));

                        if (mintPolicyIds is not null && t.Mint is not null)
                            hasMintPolicyIdMatch = t.Mint.Any(m => mintPolicyIds.Contains(m.Policy!));

                        return hasAddressMatch || hasMintPolicyIdMatch;
                    }

                    return true;
                })
                .ToList();
        }

        return ouraTransactions ?? new();
    }

    public List<OuraTxOutput> FilterTxOutputs(List<OuraTxOutput>? ouraOutputs)
    {
        if (_cardanoFilters.TxOutputFilter is not null && ouraOutputs is not null)
        {
            List<string>? addresses = _cardanoFilters.TxOutputFilter.Addresses ?? null;
            List<string>? policyIds = _cardanoFilters.TxOutputFilter.PolicyIds ?? null;

            ouraOutputs = ouraOutputs
                .Where(o =>
                {
                    if (addresses is not null || policyIds is not null)
                    {
                        bool hasAddressMatch = false;
                        bool hasPolicyIdMatch = false;

                        if (addresses is not null)
                            hasAddressMatch = addresses.Contains(o.Address!);

                        if (policyIds is not null && o.Assets is not null)
                            hasPolicyIdMatch = o.Assets.Any(a => policyIds.Contains(a.Policy!));

                        return hasAddressMatch || hasPolicyIdMatch;
                    }

                    return true;
                })
                .ToList();
        }

        return ouraOutputs ?? new();
    }

    public List<OuraAssetEvent> FilterAssets(List<OuraAssetEvent> ouraAssets)
    {
        if (_cardanoFilters.AssetFilter is not null && ouraAssets is not null)
        {
            List<string>? addresses = _cardanoFilters.AssetFilter.Addresses ?? null;
            List<string>? policyIds = _cardanoFilters.AssetFilter.PolicyIds ?? null;

            ouraAssets = ouraAssets
                .Where(a =>
                {
                    if (addresses is not null || policyIds is not null)
                    {
                        bool hasAddressMatch = false;
                        bool hasPolicyIdMatch = false;

                        if (addresses is not null)
                            hasAddressMatch = addresses.Contains(a.Address!);

                        if (policyIds is not null)
                            hasPolicyIdMatch = policyIds.Contains(a.PolicyId);

                        return hasAddressMatch || hasPolicyIdMatch;
                    }

                    return true;
                })
                .ToList();
        }

        return ouraAssets ?? new();
    }
}