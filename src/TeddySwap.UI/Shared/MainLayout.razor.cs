using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using TeddySwap.Common.Services;
using TeddySwap.UI.Models;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Shared;

public partial class MainLayout
{
    protected bool IsDrawerOpen { get; set; } = false;
    protected MudTheme Theme { get; set; } = new()
    {
        PaletteDark = new PaletteDark()
        {
            Primary = new MudColor("rgb(3, 105, 161)"),
            PrimaryDarken = "rgb(6, 80, 120)",
            Secondary = new MudColor("rgb(3, 105, 161)"),
            SecondaryDarken = "rgb(3, 105, 161)",
            Surface = new MudColor("rgba(24, 24, 27, 0.7)"),
            BackgroundGrey = new MudColor("rgb(24, 24, 27)"),
            Tertiary = new MudColor("rgba(15, 23, 42, 0.5)"),
            TextPrimary = new MudColor("#FFFFFF"),
            TextSecondary = new MudColor("rgb(161,161,170)"),
            DrawerBackground = new MudColor("rgb(38, 74, 94)"),
            DrawerText = new MudColor("#FFFFFF")
        },
        Typography = new Typography()
        {
            Default = new Default()
            {
                FontFamily = new string[] { "Montserrat", "sans-serif" }
            }
        }
    };

    protected void ToggleDrawer() => IsDrawerOpen = !IsDrawerOpen;

    [Inject]
    protected CardanoWalletService? CardanoWalletService { get; set; }

    [Inject]
    protected HeartBeatService? HeartBeatService { get; set; }

    [Inject]
    protected ISnackbar? Snackbar { get; set; }

    [Inject]
    protected ILogger<MainLayout>? Logger { get; set; }

    protected IEnumerable<CardanoWallet> CardanoWallets { get; set; } = new List<CardanoWallet>();
    protected bool IsWalletDialogShown { get; set; } = false;
    protected DialogOptions WalletDialogOptions => new()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true
    };
    protected bool IsWalletConnected => !string.IsNullOrEmpty(CardanoWalletService?.ConnectedAddress);
    protected string ConnectedAddress => (IsWalletConnected ? CardanoWalletService?.ConnectedAddress : string.Empty) ?? string.Empty;
    protected bool IsLoaded { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(CardanoWalletService);
                await CardanoWalletService.LoadStateFromStorageAsync();
            }
            catch
            {
                // @TODO log error
            }

            IsLoaded = true;
            await InvokeAsync(StateHasChanged);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected async Task OnConnectWalletShowClicked()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(CardanoWalletService);
            CardanoWallets = (await CardanoWalletService.GetAvailableWalletsAsync()).OrderBy(a => a.Name);
            IsWalletDialogShown = true;
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error Getting Available Wallets!");
        }
    }

    protected async Task OnConnectWalletClicked(CardanoWallet wallet)
    {
        ArgumentNullException.ThrowIfNull(CardanoWalletService);
        ArgumentNullException.ThrowIfNull(Snackbar);
        bool connectionResult = await CardanoWalletService.EnableAsync(wallet);
        if (!connectionResult)
        {
            Snackbar.Add("Wallet connection failed, make sure you are connected to the correct network.", Severity.Error);
        }
        else
        {
            Snackbar.Add("Wallet connected succesfully!", Severity.Success);
            IsWalletDialogShown = false;
        }
        await InvokeAsync(StateHasChanged);
    }

    protected async Task OnWalletDisconnect()
    {
        ArgumentNullException.ThrowIfNull(CardanoWalletService);
        await CardanoWalletService.DisconnectAsync();
        await InvokeAsync(StateHasChanged);
    }
}
