using Microsoft.EntityFrameworkCore;
using TeddySwap.Data.Models.Reducers;

namespace TeddySwap.Data.Services;

public class TransactionDataService(IDbContextFactory<TeddySwapDbContext> dbContextFactory)
{
    public async Task<ulong> GetTransactionIdConfirmationsAsync(string txId)
    {
        await using var dbContext = dbContextFactory.CreateDbContext();
        var output = await dbContext.TransactionOutputs
            .Where(t => t.Id == txId)
            .FirstOrDefaultAsync();

        if (output is null)
        {
            return 0;
        }

        var latestBlock = await dbContext.Blocks
            .OrderByDescending(b => b.Slot)
            .FirstOrDefaultAsync();

        var blockAtOutputSlot = await dbContext.Blocks
            .Where(b => b.Slot == output.Slot)
            .FirstOrDefaultAsync();
        if (latestBlock is null || blockAtOutputSlot is null) return 0;
        return latestBlock.Number - blockAtOutputSlot.Number;
    }
}