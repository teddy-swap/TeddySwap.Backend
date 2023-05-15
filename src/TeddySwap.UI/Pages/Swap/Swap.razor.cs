using MudBlazor;
using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Services;
using TeddySwap.UI.Models;
using System.Text.Json;
using System.ComponentModel;
using Microsoft.AspNetCore.WebUtilities;

namespace TeddySwap.UI.Pages.Swap;

public partial class Swap
{
    [Inject]
    private IDialogService? DialogService { get; set; }

    [Inject]
    protected new AppStateService? AppStateService { get; set; }

    [Inject]
    protected TeddySwapCalculatorService? TeddySwapCalculatorService { get; set; }

    [Inject]
    protected CardanoWalletService? CardanoWalletService { get; set; }

    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    private IEnumerable<Token>? Tokens { get; set; }
    private decimal _priceImpactValue;
    private decimal PriceImpactValue
    {
        get
        {
            ArgumentNullException.ThrowIfNull(AppStateService);
            ArgumentNullException.ThrowIfNull(TeddySwapCalculatorService);
            return TeddySwapCalculatorService.CalculatePriceImpact(AppStateService.FromValue);
        }
        set =>  _priceImpactValue = value;
    }
    private bool _isPanelExpanded { get; set; } = false;
    private bool _areInputsSwapped { get; set; } = false;
    private bool _isChartButtonClicked { get; set; } = false;
    private bool _isTokenTwoSelected { get; set; } = false;
    private Token _swapTokenOne { get; set; } = new();
    private Token _swapTokenTwo { get; set; } = new();

    protected override void OnInitialized()
    { 
        ArgumentNullException.ThrowIfNull(AppStateService);
        ArgumentNullException.ThrowIfNull(NavigationManager);
        string tokensJson = File.ReadAllText("./wwwroot/tokens.json");
        ArgumentException.ThrowIfNullOrEmpty(tokensJson);
        Tokens = JsonSerializer.Deserialize<IEnumerable<Token>>(tokensJson);

        Uri uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        string? tokenOneQueryString = QueryHelpers.ParseQuery(uri.Query).GetValueOrDefault("tokenOne");
        string? tokenTwoQueryString = QueryHelpers.ParseQuery(uri.Query).GetValueOrDefault("tokenTwo");

        if (!string.IsNullOrEmpty(tokenOneQueryString)) _swapTokenOne = JsonSerializer.Deserialize<Token>(tokenOneQueryString);
        if (!string.IsNullOrEmpty(tokenOneQueryString)) _swapTokenTwo = JsonSerializer.Deserialize<Token>(tokenTwoQueryString);

        if (string.IsNullOrEmpty(_swapTokenOne?.Name))
        {
            AppStateService.FromCurrentlySelectedToken = Tokens?.ElementAt(1);
        }
        else
        {
            AppStateService.FromCurrentlySelectedToken = _swapTokenOne;
            AppStateService.ToCurrentlySelectedToken = _swapTokenTwo;
        }
        
        AppStateService.PropertyChanged += OnAppStatePropertyChanged;
    }

    private async void OnAppStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        => await InvokeAsync(StateHasChanged);

    private void HandleTokenTwoSelected(Token token)
    {
        ArgumentNullException.ThrowIfNull(AppStateService);
        AppStateService.ToCurrentlySelectedToken = token;
        _isTokenTwoSelected = true;
    }
    
    private void OpenSwapSettingsDialog()
    {
        ArgumentNullException.ThrowIfNull(DialogService);
        var options = new DialogOptions { CloseOnEscapeKey = true };
        DialogService.Show<SwapSettingsDialog>("Swap Settings",  options);
    }

    private void OpenConfirmSwapDialog()
    {
        ArgumentNullException.ThrowIfNull(DialogService);
        var options = new DialogOptions { CloseOnEscapeKey = true };
        DialogService.Show<ConfirmSwapDialog>("Confirm swap", options);
    }

    private void ToggleChart() => _isChartButtonClicked = !_isChartButtonClicked;

    private void SwapInputs() => _areInputsSwapped = !_areInputsSwapped;

    private void ToggleExpansionPanel() => _isPanelExpanded = !_isPanelExpanded;

    private string GetPriceImpactValueClass()
    {
        if (PriceImpactValue < 3) return "text-[var(--mud-palette-success)]";
        if (PriceImpactValue < 5) return "text-[var(--mud-palette-warning)]";
        return "text-[var(--mud-palette-error)]";
    }
}
