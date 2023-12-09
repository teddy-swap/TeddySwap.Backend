using Microsoft.EntityFrameworkCore;
using PallasDotnet.Models;
using TeddySwap.Data;

namespace TeddySwap.Sync.Reducers;

public class LovelaceByAddressReducer(IDbContextFactory<TeddySwapDbContext> dbContext, ILogger<LovelaceByAddressReducer> logger) : IReducer
{
    private readonly IDbContextFactory<TeddySwapDbContext> _dbContext = dbContext;
    private readonly ILogger<LovelaceByAddressReducer> _logger = logger;

    public async Task RollForwardAsync(NextResponse response)
    {
        
    }

    public async Task RollBackwardAsync(NextResponse response)
    {
    }
}