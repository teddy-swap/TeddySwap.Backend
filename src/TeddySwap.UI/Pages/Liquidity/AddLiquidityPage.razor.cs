using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.UI.Models;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class AddLiquidityPage
{
    [Inject]
    public AppStateService? AppStateService { get; set; }

    private IEnumerable<Token>? Tokens { get; set; }

    private MudChip? _selectedChip { get; set; }

    protected override void OnInitialized()
    { 
        string tokensJson = File.ReadAllText("./wwwroot/tokens.json");
        ArgumentException.ThrowIfNullOrEmpty(tokensJson);
        Tokens = JsonSerializer.Deserialize<IEnumerable<Token>>(tokensJson);

        if (AppStateService is not null)
        {
            AppStateService.PropertyChanged += OnAppStatePropertyChanged;
            AppStateService.FromCurrentlySelectedToken = Tokens?.ElementAt(0);
            AppStateService.ToCurrentlySelectedToken = Tokens?.ElementAt(2);
        }
    }

    private async void OnAppStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        => await InvokeAsync(StateHasChanged);

}
