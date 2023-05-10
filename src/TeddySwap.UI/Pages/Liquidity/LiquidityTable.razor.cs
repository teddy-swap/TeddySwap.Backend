using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class LiquidityTable
{
    [Parameter]
    public IEnumerable<LiquidityData>? LiquidityData { get; set; }

    public IEnumerable<Token>? _tokens { get; set; }
    private string? _searchValue { get; set; }
    private Token? _currentlySelectedToken { get; set; }
    public TokenPair? SelectedRowTokenPair { get; set; }

    protected override void OnInitialized()
    {
        string tokensJson = File.ReadAllText("./wwwroot/tokens.json");
        ArgumentException.ThrowIfNullOrEmpty(tokensJson);
        _tokens = JsonSerializer.Deserialize<IEnumerable<Token>>(tokensJson);
    }

    private void HandleSelectedToken(Token token)
    {
        _searchValue = token.Name;
        _currentlySelectedToken = token;
        StateHasChanged();
    }

    private void HandleSearchValueChanged(string newSearchValue)
    {
        _searchValue = newSearchValue;
        if (_searchValue is not null && _searchValue != _currentlySelectedToken?.Name)
            _currentlySelectedToken = null;
    }

    private void ExpandRow(int num)
	{
        ArgumentNullException.ThrowIfNull(LiquidityData);
        LiquidityData selectedData = LiquidityData.First(d => d.Number == num);
        selectedData.ShowDetails = !selectedData.ShowDetails;
	}
    private void SelectRowTokenPair(int num)
    {   
        ArgumentNullException.ThrowIfNull(LiquidityData);
        SelectedRowTokenPair = LiquidityData.First(d => d.Number == num).TokenPair;
    }

    private IEnumerable<LiquidityData>? _filteredData =>
        string.IsNullOrEmpty(_searchValue)
            ? LiquidityData
            : LiquidityData?.Where(
                d => d.TokenPair.Tokens.Token1.Name.ToLower().Contains(_searchValue.ToLower()) || 
                     d.TokenPair.Tokens.Token2.Name.ToLower().Contains(_searchValue.ToLower())
            );
}
