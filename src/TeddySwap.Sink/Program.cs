using Blockfrost.Api.Extensions;
using CardanoSharp.Koios.Client;
using Microsoft.EntityFrameworkCore;
using Refit;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Sink.Extensions;
using TeddySwap.Sink.Filters;
using TeddySwap.Sink.Models;
using TeddySwap.Sink.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string hostname = builder.Configuration["DBSYNC_POSTGRESQL_HOSTNAME"] ?? "";
string port = builder.Configuration["DBSYNC_POSTGRESQL_PORT"] ?? "";
string user = builder.Configuration["DBSYNC_POSTGRESQL_USER"] ?? "";
string password = builder.Configuration["DBSYNC_POSTGRESQL_PASSWORD"] ?? "";
string database = builder.Configuration["DBSYNC_POSTGRESQL_DATABASE"] ?? "";
string connectionString = $"Host={hostname};Database={database};Username={user};Password={password};Port={port}";
string koiosEndpoint = builder.Configuration["KOIOS_ENDPOINT"] ?? "https://preview.koios.rest/api/v0";

// Add services to the container.
builder.Services.AddHttpClient();
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = null);
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.MaxDepth = int.MaxValue);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPooledDbContextFactory<TeddySwapSinkCoreDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapSink")), 10);
builder.Services.AddPooledDbContextFactory<TeddySwapOrderSinkDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapSink")), 10);
builder.Services.AddPooledDbContextFactory<TeddySwapNftSinkDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapSink")), 10);
builder.Services.AddPooledDbContextFactory<TeddySwapFisoSinkDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapSink")), 10);
builder.Services.AddPooledDbContextFactory<TeddySwapBadgerAddressSinkDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapSink")), 10);
builder.Services.AddPooledDbContextFactory<CardanoDbSyncContext>(options =>
{
    if (builder.Configuration["ASPNETCORE_ENVIRONMENT"]?.ToString() != "Production")
        options.EnableSensitiveDataLogging(true);
    options.UseNpgsql(connectionString, pgOptions =>
    {
        pgOptions.EnableRetryOnFailure(3);
        pgOptions.CommandTimeout(999999);
    });
}, 10);
builder.Services.Configure<TeddySwapSinkSettings>(options => builder.Configuration.GetSection("TeddySwapSinkSettings").Bind(options));
builder.Services.Configure<CardanoFilters>(options => builder.Configuration.GetSection("CardanoFilters").Bind(options));
builder.Services.AddSingleton<CardanoFilterService>();
builder.Services.AddSingleton<CardanoService>();
builder.Services.AddSingleton<ByteArrayService>();
builder.Services.AddSingleton<CborService>();
builder.Services.AddSingleton<DatumService>();
builder.Services.AddSingleton<MetadataService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddSingleton<OuraService>();
builder.Services.AddOuraReducers();
builder.Services.AddKoios(koiosEndpoint);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Set up oura cursor
var ouraSettings = builder.Configuration.GetSection("OuraSettings");
var slot = ouraSettings.GetValue<string>("DefaultSlot");
var hash = ouraSettings.GetValue<string>("DefaultBlockHash");
var path = ouraSettings.GetValue<string>("CursorPath");
var offset = ouraSettings.GetValue<int>("Offset");

using var scopedProvider = app.Services.CreateScope();
IServiceProvider service = scopedProvider.ServiceProvider;
IDbContextFactory<TeddySwapSinkCoreDbContext> dbContextFactory = service.GetRequiredService<IDbContextFactory<TeddySwapSinkCoreDbContext>>();
using TeddySwapSinkCoreDbContext? dbContext = await dbContextFactory.CreateDbContextAsync();

if (dbContext is not null)
{
    Block? block = await dbContext.Blocks.OrderByDescending(b => b.BlockNumber).Take(offset).LastOrDefaultAsync();

    if (block is not null)
    {
        slot = block.Slot.ToString();
        hash = block.BlockHash;
    }
}

await File.WriteAllTextAsync(Path.Combine(path ?? "../../deployments/config", "cursor"), $"{slot},{hash}");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
