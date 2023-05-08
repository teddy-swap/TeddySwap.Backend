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

    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }

    private IEnumerable<Token>? Tokens { get; set; }
    private IEnumerable<int>? _defaultLiquidityPercentages { get; set; }
    private IEnumerable<double>? _defaultFeePercentages { get; set; }
    private List<Pool> _pools { get; set; } = new();
    private Pool? _currentlySelectedPool { get; set; }
    public bool _isTokenTwoSelected { get; set; } = false;
    private int _currentLiquidityPercentage { get; set; }
    private double _feePercentage { get; set; }
    public double _currentlySelectedFee { get; set; }
    private bool _isFeeUnavailable { get; set; } = false;
    private bool _createNewPool { get; set; } = false;

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
                Fee = 0.01,
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
                Fee = 0.03,
                Tvl = 509M
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
                Fee = 0.05,
                Tvl = 200M
            }
        };

        _defaultLiquidityPercentages = new List<int>() { 25, 50, 75, 100 };
        _defaultFeePercentages = new List<double>() { 0.01, 0.03, 0.05 };

        AppStateService.PropertyChanged += OnAppStatePropertyChanged;
        AppStateService.FromCurrentlySelectedToken = Tokens?.ElementAt(0);
        AppStateService.ToCurrentlySelectedToken = Tokens?.ElementAt(2);
        AppStateService.LiquidityFeePercentage = GetMinPoolFee();
        AppStateService.LiquidityCurrentlySelectedTokenOne = new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" };
    }

    private async void OnAppStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        => await InvokeAsync(StateHasChanged);

    private void OnLiquidityBtnClicked(int value)
    {
        _currentLiquidityPercentage = value switch
        {
            25 => 25,
            50 => 50,
            75 => 75,
            _ => 100
        };
    }

    private void OnFeeBtnClicked(double value)
    {
        ArgumentNullException.ThrowIfNull(AppStateService);
        AppStateService.LiquidityFeePercentage = value switch
        {
            0.01 => 0.01,
            0.03 => 0.03,
            _ => 0.05
        };
        CheckFeeExists();
    }

    private void CheckFeeExists()
    {
        ArgumentNullException.ThrowIfNull(AppStateService);
        Pool? matchingPool = _pools.FirstOrDefault(p => p.Fee == AppStateService.LiquidityFeePercentage);
        if (matchingPool is null)
        {
            _isFeeUnavailable = true;
        }
        else
        {
            _currentlySelectedPool = matchingPool;
            _isFeeUnavailable = false;
            _createNewPool = false;
        }
    }

    private void CancelPoolCreation()
    {   
        ArgumentNullException.ThrowIfNull(AppStateService);
        AppStateService.LiquidityFeePercentage = GetMinPoolFee();
        CheckFeeExists();
    }

    private void InitializePoolCreation()
    {
        _createNewPool = true;
        _isFeeUnavailable = false;
    }

    private double GetMinPoolFee() => _pools.OrderBy(p => p.Fee).First().Fee;

    private void HandlePoolSelected(Pool pool)
    {
        ArgumentNullException.ThrowIfNull(AppStateService);
        _currentlySelectedPool = pool;
        AppStateService.LiquidityFeePercentage = _currentlySelectedPool.Fee;
    }

    private void HandleFeeValueChanged(double value)
    {
        ArgumentNullException.ThrowIfNull(AppStateService);
        AppStateService.LiquidityFeePercentage = value;
        CheckFeeExists();
    }

    private void HandleTokenTwoSelected(Token token)
    {
        ArgumentNullException.ThrowIfNull(AppStateService);
        AppStateService.LiquidityCurrentlySelectedTokenTwo = token;
        _isTokenTwoSelected = true;
        _currentlySelectedPool = _pools.ElementAt(0);
    }

    private void OpenAddLiquidityConfirmationDialog()
    {
        ArgumentNullException.ThrowIfNull(DialogService);
        var options = new DialogOptions { CloseOnEscapeKey = true };
        DialogService.Show<AddLiquidityConfirmationDialog>("", options);
    }
}
