using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class LiquidityTable
{
    [Inject]
    NavigationManager? NavigationManager { get; set; }

    [Parameter, EditorRequired]
    public IEnumerable<LiquidityData>? LiquidityData { get; set; }

    public IEnumerable<Token>? _tokens { get; set; }
    private string? _searchValue { get; set; }
    private Token? _currentlySelectedToken { get; set; }

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
        LiquidityData selectedData = GetRowData(num);
        selectedData.ShowDetails = !selectedData.ShowDetails;
	}

    private void HandleSwapBtnClicked(int num)
    {
        ArgumentNullException.ThrowIfNull(NavigationManager);
        LiquidityData selectedData = GetRowData(num);
        NavigationManager.NavigateTo($"/swap?tokenOne={JsonSerializer.Serialize(selectedData.TokenOne)}&tokenTwo={JsonSerializer.Serialize(selectedData.TokenTwo)}");
    }

    private void HandleAddLiquidityBtnClicked(int num)
    {
        ArgumentNullException.ThrowIfNull(NavigationManager);
        LiquidityData selectedData = GetRowData(num);
        NavigationManager.NavigateTo($"/liquidity/liquidity-center?tokenOne={JsonSerializer.Serialize(selectedData.TokenOne)}&tokenTwo={JsonSerializer.Serialize(selectedData.TokenTwo)}");
    }

    private LiquidityData GetRowData(int num)
    {
        ArgumentNullException.ThrowIfNull(LiquidityData);
        return LiquidityData.First(d => d.Number == num);
    }

    private IEnumerable<LiquidityData>? _filteredData =>
        string.IsNullOrEmpty(_searchValue)
            ? LiquidityData
            : LiquidityData?.Where(
                d => d.TokenOne.Name.ToLower().Contains(_searchValue.ToLower()) || 
                     d.TokenTwo.Name.ToLower().Contains(_searchValue.ToLower())
            );
}
