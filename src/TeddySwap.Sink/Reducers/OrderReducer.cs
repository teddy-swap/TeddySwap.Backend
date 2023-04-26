using System.Numerics;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TeddySwap.Common.Enums;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.Transaction)]
[DbContext(DbContextVariant.Order)]
public class OrderReducer : OuraReducerBase
{
    private readonly OrderService _orderService;

    public OrderReducer(OrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task ReduceAsync(OuraTransaction transaction, TeddySwapOrderSinkDbContext _dbContext)
    {

        if (transaction is not null &&
            transaction.Context is not null &&
            transaction.Fee is not null)
        {
            if (_dbContext is null) return;

            Block? block = await _dbContext.Blocks
                .Where(b => b.BlockHash == transaction.Context.BlockHash)
                .FirstOrDefaultAsync();

            if (block is null) throw new NullReferenceException("Block does not exist!");
            if (block.InvalidTransactions is not null && block.InvalidTransactions.ToList().Contains((ulong)transaction.Index)) return;

            Order? existingOrder = await _dbContext.Orders
                .Where(o => o.TxHash == transaction.Hash && o.Index == (ulong)transaction.Index)
                .FirstOrDefaultAsync();

            if (existingOrder is not null) return;

            Order? order = await _orderService.ProcessOrderAsync(transaction);

            if (order is not null)
            {
                order.Block = block;
                order.Blockhash = block.BlockHash;

                await _dbContext.Orders.AddAsync(order);

                if (order.OrderType == OrderType.Swap)
                {
                    await _dbContext.Prices.AddAsync(new Price()
                    {
                        TxHash = order.TxHash,
                        Index = order.Index,
                        Order = order,
                        PriceX = BigIntegerDivToDecimal(order.ReservesX, order.ReservesY, 6),
                        PriceY = BigIntegerDivToDecimal(order.ReservesY, order.ReservesX, 6),
                    });
                }

            }

            await _dbContext.SaveChangesAsync();
        }
    }

    private static decimal BigIntegerDivToDecimal(BigInteger x, BigInteger y, int precision)
    {

        var divResult = BigInteger.DivRem(x, y);

        StringBuilder result = new();
        result.Append(divResult.Quotient.ToString());

        if (divResult.Remainder != 0)
        {
            result.Append('.');

            for (int i = 0; i < precision; i++)
            {
                divResult.Remainder *= 10;
                result.Append(BigInteger.DivRem(divResult.Remainder, y).Quotient.ToString());

                if (BigInteger.DivRem(divResult.Remainder, y).Remainder == 0)
                {
                    break;
                }
            }
        }

        return decimal.Parse(result.ToString());
    }
    public async Task RollbackAsync(Block rollbackBlock, TeddySwapOrderSinkDbContext _dbContext)
    {
        if (rollbackBlock is not null)
        {
            List<Order>? orders = await _dbContext.Orders
                .Include(o => o.Block)
                .Where(o => o.Block == rollbackBlock)
                .ToListAsync();

            _dbContext.Orders.RemoveRange(orders);
            await _dbContext.SaveChangesAsync();
        }
    }
}