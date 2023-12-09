using Microsoft.EntityFrameworkCore;
using TeddySwap.Data;
using TeddySwap.Sync.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<TeddySwapDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapContext"))
);
builder.Services.AddHostedService<CardanoIndexWorker>();
var app = builder.Build();

app.MapGet("/hello", () =>
{
    return "hello";
})
.WithName("hello")
.WithOpenApi();

app.Run();
