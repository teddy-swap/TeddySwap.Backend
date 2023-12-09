using Microsoft.EntityFrameworkCore;
using TeddySwap.Data;
using TeddySwap.Sync.Reducers;
using TeddySwap.Sync.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<TeddySwapDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapContext"), x => x.MigrationsHistoryTable("__EFMigrationsHistory", "public"))
);

builder.Services.AddSingleton<ICoreReducer, BlockReducer>();
builder.Services.AddSingleton<ICoreReducer, TransactionOutputReducer>();
builder.Services.AddSingleton<IReducer, LovelaceByAddressReducer>();

builder.Services.AddHostedService<CardanoIndexWorker>();

var app = builder.Build();

app.MapGet("/hello", () =>
{
    return "hello";
})
.WithName("hello")
.WithOpenApi();

app.Run();
