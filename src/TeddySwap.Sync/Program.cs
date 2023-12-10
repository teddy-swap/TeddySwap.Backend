using Microsoft.EntityFrameworkCore;
using TeddySwap.Data;
using TeddySwap.Sync.Reducers;
using TeddySwap.Sync.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<TeddySwapDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapContext"), x => x.MigrationsHistoryTable("__EFMigrationsHistory", "public"))
);

builder.Services.AddSingleton<IBlockReducer, BlockReducer>();
builder.Services.AddSingleton<ICoreReducer, TransactionOutputReducer>();
builder.Services.AddSingleton<IReducer, TeddyYieldFarmingReducer>();

builder.Services.AddHostedService<CardanoIndexWorker>();

var app = builder.Build();

app.MapGet("/lp_states/block/{blockNumber}", async (ulong blockNumber, IDbContextFactory<TeddySwapDbContext> dbContextFactory) =>
{
    await using var dbContext = dbContextFactory.CreateDbContext();

    // Query the Block by block number
    var block = await dbContext.Blocks
        .FirstOrDefaultAsync(b => b.Number == blockNumber);

    if (block == null)
    {
        // Block not found, return an appropriate response
        return Results.NotFound($"Block with number {blockNumber} not found.");
    }

    // Query the latest LiquidityByAddressItem record for each address up to the block's slot
    var latestLiquidityStates = await dbContext.LiquidityByAddress
        .Where(item => item.Slot <= block.Slot)
        .GroupBy(item => item.Address)
        .Select(group => group.OrderByDescending(item => item.Slot).FirstOrDefault())
        .ToListAsync();

    // Transform the data as needed for the API response
    // For simplicity, returning the raw data here
    return Results.Ok(latestLiquidityStates);
})
.WithName("GetLatestLiquidityStatesByBlock")
.WithOpenApi();


app.Run();
