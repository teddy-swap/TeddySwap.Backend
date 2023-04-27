using System.Text;
using System.Text.Json;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.Transaction)]
[DbContext(DbContextVariant.Core)]
public class TransactionReducer : OuraReducerBase, IOuraCoreReducer
{
    private readonly CardanoService _cardanoService;

    public TransactionReducer(CardanoService cardanoService)
    {
        _cardanoService = cardanoService;
    }

    public async Task ReduceAsync(OuraTransaction transaction, TeddySwapSinkCoreDbContext _dbContext)
    {
        if (transaction is not null &&
            transaction.Fee is not null &&
            transaction.Hash is not null &&
            transaction.Context is not null &&
            transaction.Context.BlockHash is not null)
        {
            Transaction newTransaction = new()
            {
                Hash = transaction.Hash,
                Fee = (ulong)transaction.Fee,
                Index = (ulong)transaction.Index,
                BlockHash = transaction.Context.BlockHash,
                Metadata = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(transaction.Metadata))),
                HasCollateralOutput = transaction.HasCollateralOutput,
                IsValid = !_cardanoService.IsInvalidTransaction(transaction.Context.InvalidTransactions, (ulong)transaction.Index)
            };

            await _dbContext.Transactions.AddAsync(newTransaction);
            await _dbContext.SaveChangesAsync();
        }
    }
    public async Task RollbackAsync(Block rollbackBlock, TeddySwapSinkCoreDbContext _dbContext)
    {
        var transactions = await _dbContext.Transactions
            .Where(t => t.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.Transactions.RemoveRange(transactions);
        await _dbContext.SaveChangesAsync();
    }
}