using Microsoft.EntityFrameworkCore;
using TeddySwap.Sink.Api.Models;
using TeddySwap.Sink.Api.Services;
using TeddySwap.Sink.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string hostname = builder.Configuration["DBSYNC_POSTGRESQL_HOSTNAME"] ?? "";
string port = builder.Configuration["DBSYNC_POSTGRESQL_PORT"] ?? "";
string user = builder.Configuration["DBSYNC_POSTGRESQL_USER"] ?? "";
string password = builder.Configuration["DBSYNC_POSTGRESQL_PASSWORD"] ?? "";
string database = builder.Configuration["DBSYNC_POSTGRESQL_DATABASE"] ?? "";
string connectionString = $"Host={hostname};Database={database};Username={user};Password={password};Port={port}";

builder.Services.AddDbContextPool<CardanoDbSyncContext>(options =>
{
    if (builder.Configuration["ASPNETCORE_ENVIRONMENT"]?.ToString() != "Production")
        options.EnableSensitiveDataLogging(true);
    options.UseNpgsql(connectionString, pgOptions =>
    {
        pgOptions.EnableRetryOnFailure(3);
        pgOptions.CommandTimeout(999999);
    });
}, 10);

builder.Services.AddDbContextPool<TeddySwapSinkCoreDbContext>(options =>
{
    if (builder.Configuration["ASPNETCORE_ENVIRONMENT"]?.ToString() != "Production")
        options.EnableSensitiveDataLogging(true);
    options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapSink"), pgOptions => pgOptions.EnableRetryOnFailure(3));
}, 10);

builder.Services.AddDbContextPool<TeddySwapOrderSinkDbContext>(options =>
{
    if (builder.Configuration["ASPNETCORE_ENVIRONMENT"]?.ToString() != "Production")
        options.EnableSensitiveDataLogging(true);
    options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapOrderSink"), pgOptions => pgOptions.EnableRetryOnFailure(3));
}, 10);

builder.Services.AddDbContextPool<TeddySwapNftSinkDbContext>(options =>
{
    if (builder.Configuration["ASPNETCORE_ENVIRONMENT"]?.ToString() != "Production")
        options.EnableSensitiveDataLogging(true);
    options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapNftSink"), pgOptions => pgOptions.EnableRetryOnFailure(3));
}, 10);

builder.Services.AddDbContextPool<TeddySwapFisoSinkDbContext>(options =>
{
    if (builder.Configuration["ASPNETCORE_ENVIRONMENT"]?.ToString() != "Production")
        options.EnableSensitiveDataLogging(true);
    options.UseNpgsql(builder.Configuration.GetConnectionString("TeddySwapFisoSink"), pgOptions => pgOptions.EnableRetryOnFailure(3));
}, 10);

builder.Services.Configure<TeddySwapITNRewardSettings>(options => builder.Configuration.GetSection("TeddySwapITNRewardSettings").Bind(options));
builder.Services.Configure<TeddySwapValidatorSettings>(options => builder.Configuration.GetSection("TeddySwapValidatorSettings").Bind(options));
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<LeaderboardService>();
builder.Services.AddScoped<AssetService>();
builder.Services.AddScoped<StakeService>();
builder.Services.AddScoped<FisoRewardService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<AddressVerificationService>();
builder.Services.AddSingleton<DatumService>();
builder.Services.AddSingleton<CborService>();
builder.Services.AddApiVersioning(options => options.AssumeDefaultVersionWhenUnspecified = true).AddMvc();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
