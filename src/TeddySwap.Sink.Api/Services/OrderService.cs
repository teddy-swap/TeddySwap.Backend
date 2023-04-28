using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Models;
using TeddySwap.Common.Models.CardanoDbSync;
using TeddySwap.Common.Enums;
using TeddySwap.Common.Models.Request;
using TeddySwap.Common.Models.Response;
using TeddySwap.Sink.Api.Models;
using TeddySwap.Sink.Data;
using PeterO.Cbor2;
using CardanoSharp.Wallet.Utilities;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;

namespace TeddySwap.Sink.Api.Services;

public class OrderService
{
    private readonly TeddySwapOrderSinkDbContext _dbContext;
    private readonly TeddySwapValidatorSettings _settings;
    private readonly DatumService _datumService;

    public OrderService(
        TeddySwapOrderSinkDbContext dbContext,
        IOptions<TeddySwapValidatorSettings> settings,
        DatumService datumService)
    {
        _dbContext = dbContext;
        _settings = settings.Value;
        _datumService = datumService;
    }

    public async Task<PaginatedLiquidityResponse> GetLatestLiquidityPoolsAsync(int offset, int limit)
    {
        var liquidityPoolsQuery = _dbContext.Orders
            .OrderByDescending(o => o.Slot)
            .GroupBy(o => o.PoolNft)
            .Select(g => g.First());

        int total = await liquidityPoolsQuery.CountAsync();

        var liquidityPools = (await liquidityPoolsQuery
            .Skip(offset)
            .Take(limit)
            .ToListAsync())
            .Select(o => new LiquidityPoolResponse()
            {
                NftUnit = o.PoolNft,
                LqUnit = o.AssetLq,
                AssetXUnit = o.AssetX,
                AssetYUnit = o.AssetY,
                Fee = (1000 - o.Fee) / 100,
                ReservesX = o.ReservesX.ToString(),
                ReservesY = o.ReservesY.ToString()
            });

        return new()
        {
            Result = liquidityPools,
            TotalCount = total
        };
    }

    public async Task<PaginatedSwapOrderResponse> GetLatestSwapOrdersAsync(int offset, int limit)
    {
        var ordersQuery = _dbContext.TxOutputs
            .Where(o => o.Address == _settings.SwapAddress && o.DatumCbor != null);

        var total = await ordersQuery.CountAsync();

        var orders = await ordersQuery
            .OrderByDescending(o => o.Slot)
            .Skip(offset)
            .Take(limit)
            .GroupJoin(_dbContext.Orders.Where(o => o.OrderType == OrderType.Swap),
                output => output.TxHash + output.Index,
                order => order.OrderTxHash + order.OrderOutputIndex,
                (output, order) => new { output, order })
            .ToListAsync();

        IEnumerable<SwapOrderResponse> taggedOrders = orders.Select(o =>
        {
            byte[] swapDatumByte = Convert.FromHexString(o.output.DatumCbor!);
            SwapDatum swapDatum = _datumService.CborToSwapDatum(CBORObject.DecodeFromBytes(swapDatumByte));
            Order? order = o.order.FirstOrDefault();
            bool isExecuted = order is not null;

            return new SwapOrderResponse()
            {
                TxHash = o.output.TxHash,
                OutputIndex = o.output.Index,
                BaseAsset = swapDatum.Base.PolicyId + swapDatum.Base.Name,
                BaseAmount = swapDatum.BaseAmount.ToString(),
                Slot = o.output.Slot,
                QuoteAsset = swapDatum.Quote.PolicyId + swapDatum.Quote.Name,
                QuoteAmount = isExecuted ? order!.AssetX == swapDatum.Quote.PolicyId + swapDatum.Quote.Name ? order.OrderX.ToString() : order.OrderY.ToString() : null,
                Owner = GetBaseAddress(swapDatum.RewardPkh, swapDatum.StakePkh ?? ""),
                OrderStatus = isExecuted ? OrderStatus.Executed : OrderStatus.Pending
            };
        });

        return new()
        {
            Result = taggedOrders,
            TotalCount = total
        };
    }

    public async Task<PaginatedSwapOrderResponse> GetLatestExecutedSwapOrdersByAddressAsync(string address, int offset, int limit)
    {
        var ordersQuery = _dbContext.Orders
            .Where(o => o.OrderType == OrderType.Swap)
            .Where(o => o.UserAddress == address);

        var total = await ordersQuery.CountAsync();

        var orders = await ordersQuery
            .OrderByDescending(o => o.Slot)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        IEnumerable<SwapOrderResponse> taggedOrders = orders.Select(o =>
        {
            return new SwapOrderResponse()
            {
                TxHash = o.TxHash,
                OutputIndex = o.Index,
                BaseAsset = o.OrderBase,
                BaseAmount = o.AssetX == o.OrderBase ? o.OrderX.ToString() : o.OrderY.ToString(),
                Slot = o.Slot,
                QuoteAsset = o.OrderBase == o.AssetX ? o.AssetY : o.AssetX,
                QuoteAmount = o.OrderBase == o.AssetX ? o.OrderY.ToString() : o.OrderX.ToString(),
                Owner = o.UserAddress,
                OrderStatus = OrderStatus.Executed
            };
        });

        return new()
        {
            Result = taggedOrders,
            TotalCount = total
        };
    }

    private string GetBaseAddress(string paymentHash, string stakeHash)
    {
        byte[] payment = Convert.FromHexString(paymentHash);
        byte[] stake = Convert.FromHexString(stakeHash ?? "");
        string baseAddress = AddressUtility.GetBaseAddress(payment, stake, _settings.NetworkType).ToString();

        return baseAddress;
    }
}