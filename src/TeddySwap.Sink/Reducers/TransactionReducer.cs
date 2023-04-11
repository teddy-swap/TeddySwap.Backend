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
    private readonly ILogger<TransactionReducer> _logger;
    private readonly IDbContextFactory<TeddySwapSinkCoreDbContext> _dbContextFactory;
    private readonly ByteArrayService _byteArrayService;
    private readonly TeddySwapSinkSettings _settings;

    public TransactionReducer(
        ILogger<TransactionReducer> logger,
        IDbContextFactory<TeddySwapSinkCoreDbContext> dbContextFactory,
        ByteArrayService byteArrayService,
        IOptions<TeddySwapSinkSettings> settings
        )
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _byteArrayService = byteArrayService;
        _settings = settings.Value;
    }

    public async Task ReduceAsync(OuraTransaction transaction)
    {
        if (transaction is not null &&
            transaction.Context is not null &&
            transaction.Fee is not null &&
            transaction.Hash is not null)
        {
            using TeddySwapSinkCoreDbContext _dbContext = await _dbContextFactory.CreateDbContextAsync();

            Block? block = await _dbContext.Blocks
                .Where(b => b.BlockHash == transaction.Context.BlockHash)
                .FirstOrDefaultAsync();

            if (block is null) throw new NullReferenceException("Block does not exist!");

            Transaction? existingTransaction = await _dbContext.Transactions
                .Where(t => t.Hash == transaction.Hash)
                .FirstOrDefaultAsync();

            if (existingTransaction is not null) return;

            string? metadata = null;
            try
            {
                metadata = JsonSerializer.Serialize(transaction.Metadata).Replace("\u0000", "\uFFFF");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }

            Transaction newTransaction = new()
            {
                Hash = transaction.Hash,
                Fee = (ulong)transaction.Fee,
                Index = (ulong)transaction.Index,
                Block = block,
                Blockhash = block.BlockHash,
                Metadata = metadata,
                HasCollateralOutput = transaction.HasCollateralOutput
            };

            // Record collateral input if available
            if (transaction.CollateralInputs is not null)
            {
                List<CollateralTxIn> collateralInputs = new();
                foreach (OuraTxInput ouraTxInput in transaction.CollateralInputs)
                {
                    collateralInputs.Add(new CollateralTxIn()
                    {
                        TxHash = transaction.Hash,
                        Transaction = newTransaction,
                        TxOutputHash = ouraTxInput.TxHash,
                        TxOutputIndex = ouraTxInput.Index
                    });
                }
                await _dbContext.CollateralTxIns.AddRangeAsync(collateralInputs);
            }

            // If Transaction is invalid record, collateral output
            if (block.InvalidTransactions is not null &&
                transaction.CollateralOutput is not null &&
                transaction.CollateralOutput.Address is not null &&
                block.InvalidTransactions.ToList().Contains((ulong)transaction.Index))
            {
                CollateralTxOut collateralOutput = new()
                {
                    Transaction = newTransaction,
                    TxIndex = (ulong)transaction.Index,
                    TxHash = transaction.Hash,
                    Index = 0,
                    Address = transaction.CollateralOutput.Address,
                    Amount = transaction.CollateralOutput.Amount
                };

                await _dbContext.CollateralTxOuts.AddAsync(collateralOutput);
            }

            await _dbContext.Transactions.AddAsync(newTransaction);
            await _dbContext.SaveChangesAsync();
        }
    }
    public async Task RollbackAsync(Block rollbackBlock) => await Task.CompletedTask;
}