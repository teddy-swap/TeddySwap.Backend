using MudBlazor;
using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Services;
using TeddySwap.UI.Models;
using System.Text.Json;
using System.ComponentModel;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace TeddySwap.UI.Pages.Swap;

public partial class Swap
{
    [Inject]
    private IDialogService? DialogService { get; set; }

    [Inject]
    protected new AppStateService? AppStateService { get; set; }

    [Inject]
    protected CardanoWalletService? CardanoWalletService { get; set; }

    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    private IEnumerable<Token>? Tokens { get; set; }
    private double _priceImpactValue;
    private double PriceImpactValue
    {
        get
        {
            ArgumentNullException.ThrowIfNull(AppStateService);
            return SwapCalculatorService.CalculatePriceImpact(AppStateService.FromValue);
        }
        set =>  _priceImpactValue = value;
    }
    private bool _isPanelExpanded { get; set; } = false;
    private bool _areInputsSwapped { get; set; } = false;
    private bool _isChartButtonClicked { get; set; } = false;
    private Token TokenOne { get; set; } = new();
    private Token TokenTwo { get; set; } = new();
    private TokenPair TokenPair { get; set; } = new();

    protected override void OnInitialized()
    { 
        string tokensJson = File.ReadAllText("./wwwroot/tokens.json");
        ArgumentException.ThrowIfNullOrEmpty(tokensJson);
        Tokens = JsonSerializer.Deserialize<IEnumerable<Token>>(tokensJson);

        ArgumentNullException.ThrowIfNull(NavigationManager);
        Uri uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        string? tokenOneQueryString = QueryHelpers.ParseQuery(uri.Query).GetValueOrDefault("tokenOne");
        string? tokenTwoQueryString = QueryHelpers.ParseQuery(uri.Query).GetValueOrDefault("tokenTwo");

        if (!string.IsNullOrEmpty(tokenOneQueryString)) TokenOne = JsonSerializer.Deserialize<Token>(tokenOneQueryString);
        if (!string.IsNullOrEmpty(tokenOneQueryString)) TokenTwo = JsonSerializer.Deserialize<Token>(tokenTwoQueryString);

        ArgumentNullException.ThrowIfNull(AppStateService);

        if (string.IsNullOrEmpty(TokenOne?.Name))
        {
            AppStateService.FromCurrentlySelectedToken = Tokens?.ElementAt(0);
            AppStateService.ToCurrentlySelectedToken = Tokens?.ElementAt(2);
        }
        else
        {
            AppStateService.FromCurrentlySelectedToken = TokenOne;
            AppStateService.ToCurrentlySelectedToken = TokenTwo;
        }
        
        AppStateService.PropertyChanged += OnAppStatePropertyChanged;
    }

    private async void OnAppStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        => await InvokeAsync(StateHasChanged);
    
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
