using Blazored.LocalStorage;
using MudBlazor.Services;
using TeddySwap.Common.Services;
using TeddySwap.UI.Services;
using TeddySwap.UI.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry((o) =>
{
    o.ConnectionString = builder.Configuration.GetConnectionString("APPLICATIONINSIGHTS");
});

builder.Services.AddBlazoredLocalStorage();
// Add services to the container.
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.HideTransitionDuration = 100;
    config.SnackbarConfiguration.ShowTransitionDuration = 100;
});

builder.Services.AddRazorPages();
builder.Services.
    AddServerSideBlazor(o =>
    {
        o.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(2);
        o.MaxBufferedUnacknowledgedRenderBatches = int.MaxValue;
    })
    .AddHubOptions((o) =>
    {
        o.MaximumReceiveMessageSize = long.MaxValue;
        o.StreamBufferCapacity = int.MaxValue;
        o.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
        o.HandshakeTimeout = TimeSpan.FromMinutes(2);
    });

builder.Services.AddHostedService<HeartBeatWorker>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ConfigService>();
builder.Services.AddSingleton<HeartBeatService>();
builder.Services.AddSingleton<SinkService>();
builder.Services.AddSingleton<QueryService>();
builder.Services.AddSingleton<NftService>();
builder.Services.AddSingleton<RewardService>();
builder.Services.AddScoped<IconsService>();
builder.Services.AddScoped<CardanoWalletService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
