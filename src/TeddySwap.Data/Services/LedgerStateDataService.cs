using Microsoft.EntityFrameworkCore;
using TeddySwap.Data.Models.Reducers;

namespace TeddySwap.Data.Services;

public class LedgerStateDataService(IDbContextFactory<TeddySwapDbContext> dbContextFactory)
{
    public async Task<LedgerStateByAddress?> LedgerStateByAddressAsync(string address)
    {
        await using var dbContext = dbContextFactory.CreateDbContext();
        return await dbContext.LedgerStateByAddress
            .Where(l => l.Address == address)
            .OrderByDescending(l => l.Slot)
            .FirstOrDefaultAsync();
    }
}