using System.Numerics;
using Microsoft.Extensions.Options;
using PeterO.Cbor2;
using TeddySwap.Common.Enums;
using TeddySwap.Common.Models;

namespace TeddySwap.Sink.Api.Services;

public class DatumService
{
    private readonly CborService _cborService;
    private readonly TeddySwapValidatorSettings _settings;
    public DatumService(CborService cborService, IOptions<TeddySwapValidatorSettings> settings)
    {
        _cborService = cborService;
        _settings = settings.Value;
    }

    public PoolDatum CborToPoolDatum(CBORObject? cbor)
    {
        if (cbor is null || cbor.Count != 5)
        {
            throw new InvalidDataException("Not a valid pool cbor");
        }

        PoolDatum poolDatum = new()
        {
            Nft = CborToAssetClass(cbor[0]),
            ReserveX = CborToAssetClass(cbor[1]),
            ReserveY = CborToAssetClass(cbor[2]),
            Lq = CborToAssetClass(cbor[3]),
            Fee = (uint)_cborService.DecodeValueByCborType(cbor[4])
        };

        return poolDatum;
    }

    public DepositDatum CborToDepositDatum(CBORObject? cbor)
    {
        if (cbor is null || cbor.Count != 8)
        {
            throw new InvalidDataException("Not a valid deposit cbor");
        }

        DepositDatum depositDatum = new()
        {
            Nft = CborToAssetClass(cbor[0]),
            ReserveX = CborToAssetClass(cbor[1]),
            ReserveY = CborToAssetClass(cbor[2]),
            Lq = CborToAssetClass(cbor[3]),
            ExFee = (uint)_cborService.DecodeValueByCborType(cbor[4]),
            RewardPkh = ((string)_cborService.DecodeValueByCborType(cbor[5])).ToLower(),
            StakePkh = ((string)_cborService.DecodeValueByCborType(cbor[6][0])).ToLower(),
            CollateralAda = (uint)_cborService.DecodeValueByCborType(cbor[7])
        };

        return depositDatum;
    }

    public RedeemDatum CborToRedeemDatum(CBORObject? cbor)
    {
        if (cbor is null || cbor.Count != 7)
        {
            throw new InvalidDataException("Not a valid redeem cbor");
        }

        RedeemDatum redeemDatum = new()
        {
            Nft = CborToAssetClass(cbor[0]),
            ReserveX = CborToAssetClass(cbor[1]),
            ReserveY = CborToAssetClass(cbor[2]),
            Lq = CborToAssetClass(cbor[3]),
            ExFee = (uint)_cborService.DecodeValueByCborType(cbor[4]),
            RewardPkh = ((string)_cborService.DecodeValueByCborType(cbor[5])).ToLower(),
            StakePkh = ((string)_cborService.DecodeValueByCborType(cbor[6][0])).ToLower()
        };

        return redeemDatum;
    }

    public SwapDatum CborToSwapDatum(CBORObject? cbor)
    {
        if (cbor is null || cbor.Count != 10)
        {
            throw new InvalidDataException("Not a valid swap cbor");
        }

        SwapDatum swapDatum = new()
        {
            Base = CborToAssetClass(cbor[0]),
            Quote = CborToAssetClass(cbor[1]),
            Nft = CborToAssetClass(cbor[2]),
            Fee = (uint)_cborService.DecodeValueByCborType(cbor[3]),
            ExFeePerTokenNum = BigInteger.Parse(_cborService.DecodeValueByCborType(cbor[4]).ToString() ?? "0"),
            ExFeePerTokenDen = BigInteger.Parse(_cborService.DecodeValueByCborType(cbor[5]).ToString() ?? "0"),
            RewardPkh = ((string)_cborService.DecodeValueByCborType(cbor[6])).ToLower(),
            StakePkh = ((string)_cborService.DecodeValueByCborType(cbor[7][0])).ToLower(),
            BaseAmount = BigInteger.Parse(_cborService.DecodeValueByCborType(cbor[8]).ToString() ?? "0"),
            MinQuoteAmount = BigInteger.Parse(_cborService.DecodeValueByCborType(cbor[9]).ToString() ?? "0")
        };

        return swapDatum;
    }

    public AssetClass CborToAssetClass(CBORObject? cbor)
    {
        if (cbor is null) return new AssetClass();

        var assetClass = new AssetClass
        {
            PolicyId = ((string)_cborService.DecodeValueByCborType(cbor[0])).ToLower(),
            Name = ((string)_cborService.DecodeValueByCborType(cbor[1])).ToLower()
        };

        assetClass.PolicyId = String.IsNullOrEmpty(assetClass.PolicyId) ? "lovelace" : assetClass.PolicyId;

        return assetClass;
    }

    public OrderType GetOrderType(string validatorAddr)
    {
        if (validatorAddr == _settings.DepositAddress)
        {
            return OrderType.Deposit;
        }
        else if (validatorAddr == _settings.RedeemAddress)
        {
            return OrderType.Redeem;
        }
        else if (validatorAddr == _settings.SwapAddress)
        {
            return OrderType.Swap;
        }
        else
        {
            return OrderType.Unknown;
        }
    }
}