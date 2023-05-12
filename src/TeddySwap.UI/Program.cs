using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using TeddySwap.Common.Services;
using TeddySwap.UI.Services;
using TeddySwap.UI.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddBlazoredLocalStorage();
// Add services to the container.
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.HideTransitionDuration = 100;
    config.SnackbarConfiguration.ShowTransitionDuration = 100;
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHostedService<HeartBeatWorker>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ConfigService>();
builder.Services.AddSingleton<HeartBeatService>();
builder.Services.AddSingleton<SinkService>();
builder.Services.AddSingleton<QueryService>();
builder.Services.AddSingleton<NftService>();
builder.Services.AddSingleton<RewardService>();
builder.Services.AddScoped<AppStateService>();
builder.Services.AddScoped<CardanoWalletService>();
builder.Services.AddScoped<TeddySwapCalculatorService>();

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
