using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.UI.Models;
using TeddySwap.UI.Services;
using TeddySwap.UI.Shared;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class AddLiquidityPage
{
    [Inject]
    public AppStateService? AppStateService { get; set; }

    [Inject]
    public IDialogService? DialogService { get; set; }

    private IEnumerable<Token>? Tokens { get; set; }

    private List<Pool> _pools { get; set; } = new();

    private Pool? _currentlySelectedPool { get; set; }

    public bool _isTokenTwoSelected { get; set; } = false;

    protected override void OnInitialized()
    { 
        string tokensJson = File.ReadAllText("./wwwroot/tokens.json");
        ArgumentException.ThrowIfNullOrEmpty(tokensJson);
        Tokens = JsonSerializer.Deserialize<IEnumerable<Token>>(tokensJson);
        ArgumentNullException.ThrowIfNull(AppStateService);

        _pools = new List<Pool>()
        {
            new()
            {
                Pair = new TokenPair() 
                { 
                    Tokens = (
                        new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" },
                        new Token() { Name = "DJED", Logo = "../images/tokens/djed.png" }
                    )
                },
                Fee = 0.08M,
                Tvl = 26.29M
            },
            new()
            {
                Pair = new TokenPair()
                { 
                    Tokens = (
                        new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" },
                        new Token() { Name = "DJED", Logo = "../images/tokens/djed.png" }
                    )
                },
                Fee = 0.06M,
                Tvl = 500M
            },
            new()
            {
                Pair = new TokenPair()
                { 
                    Tokens = (
                        new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" },
                        new Token() { Name = "DJED", Logo = "../images/tokens/djed.png" }
                    )
                },
                Fee = 0.1M,
                Tvl = 200M
            }
        };

        AppStateService.PropertyChanged += OnAppStatePropertyChanged;
        AppStateService.FromCurrentlySelectedToken = Tokens?.ElementAt(0);
        AppStateService.ToCurrentlySelectedToken = Tokens?.ElementAt(2);
        AppStateService.LiquidityPercentageValue = 25;
        AppStateService.AddLiquidityCurrentlySelectedTokenOne = new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" };
    }

    private async void OnAppStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        => await InvokeAsync(StateHasChanged);

    private void OnLiquidityBtnClicked(int value)
    {
        ArgumentNullException.ThrowIfNull(AppStateService);
        AppStateService.LiquidityPercentageValue = value switch
        {
            25 => 25,
            50 => 50,
            75 => 75,
            _ => 100
        };
    }

    private void HandleTokenTwoSelection(Token token)
    {
        ArgumentNullException.ThrowIfNull(AppStateService);
        AppStateService.AddLiquidityCurrentlySelectedTokenTwo = token;
        _isTokenTwoSelected = true;
        _currentlySelectedPool = _pools.ElementAt(0);
    }

    private void OpenDialog()
    {
        ArgumentNullException.ThrowIfNull(DialogService);

        var options = new DialogOptions { CloseOnEscapeKey = true };
  
        DialogService.Show<AddLiquidityConfirmationDialog>("Confirm Add Liquidity", options);
    }
}
