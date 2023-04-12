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
using TeddySwap.Sink.Models.Oura;
using TeddySwap.Sink.Services;

namespace TeddySwap.Sink.Reducers;

[OuraReducer(OuraVariant.Transaction)]
public class TransactionReducer : OuraReducerBase, IOuraCoreReducer
{
    private readonly IDbContextFactory<TeddySwapSinkCoreDbContext> _dbContextFactory;
    private readonly CardanoService _cardanoService;

    public TransactionReducer(IDbContextFactory<TeddySwapSinkCoreDbContext> dbContextFactory, CardanoService cardanoService)
    {
        _dbContextFactory = dbContextFactory;
        _cardanoService = cardanoService;
    }

    public async Task ReduceAsync(OuraTransaction transaction)
    {
        if (transaction is not null &&
            transaction.Fee is not null &&
            transaction.Hash is not null &&
            transaction.Context is not null &&
            transaction.Context.BlockHash is not null)
        {
            using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

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

            // @TODO: Move Collaterals to its own reducers

            // // Record collateral input if available
            // if (transaction.CollateralInputs is not null)
            // {
            //     List<CollateralTxIn> collateralInputs = new();
            //     foreach (OuraTxInput ouraTxInput in transaction.CollateralInputs)
            //     {
            //         collateralInputs.Add(new CollateralTxIn()
            //         {
            //             TxHash = transaction.Hash,
            //             Transaction = newTransaction,
            //             TxOutputHash = ouraTxInput.TxHash,
            //             TxOutputIndex = ouraTxInput.Index
            //         });
            //     }
            //     await _dbContext.CollateralTxIns.AddRangeAsync(collateralInputs);
            // }

            // // If Transaction is invalid record, collateral output
            // if (block.InvalidTransactions is not null &&
            //     transaction.CollateralOutput is not null &&
            //     transaction.CollateralOutput.Address is not null &&
            //     block.InvalidTransactions.ToList().Contains((ulong)transaction.Index))
            // {
            //     CollateralTxOut collateralOutput = new()
            //     {
            //         Transaction = newTransaction,
            //         TxIndex = (ulong)transaction.Index,
            //         TxHash = transaction.Hash,
            //         Index = 0,
            //         Address = transaction.CollateralOutput.Address,
            //         Amount = transaction.CollateralOutput.Amount
            //     };

            //     await _dbContext.CollateralTxOuts.AddAsync(collateralOutput);
            // }

            await _dbContext.Transactions.AddAsync(newTransaction);
            await _dbContext.SaveChangesAsync();
        }
    }
    public async Task RollbackAsync(Block rollbackBlock)
    {
        using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var transactions = await _dbContext.Transactions
            .Where(t => t.BlockHash == rollbackBlock.BlockHash)
            .ToListAsync();

        _dbContext.Transactions.RemoveRange(transactions);
        await _dbContext.SaveChangesAsync();
    }
}