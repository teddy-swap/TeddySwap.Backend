using CardanoSharp.Wallet;
using Microsoft.EntityFrameworkCore;
using TeddySwap.Data;
using TeddySwap.Data.Services;
using TeddySwap.Distributor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<TeddySwapDbContext>(options =>
{
    options
    .UseNpgsql(
        builder.Configuration
        .GetConnectionString("TeddySwapContext"),
            x =>
            {
                x.MigrationsHistoryTable(
                    "__EFMigrationsHistory",
                    builder.Configuration.GetConnectionString("TeddySwapContextSchema")
                );
            }
        );
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddHostedService<DistributionWorker>();

// Cardano Sharp
builder.Services.AddSingleton<MnemonicService>();

builder.Services.AddSingleton<BlockDataService>();
builder.Services.AddSingleton<LedgerStateDataService>();
builder.Services.AddSingleton<YieldFarmingDataService>();
builder.Services.AddSingleton<TransactionDataService>();

var app = builder.Build();

app.MapGet("/hello", () => "Hello World!")
.WithName("HelloWorld")
.WithOpenApi();

app.Run();
