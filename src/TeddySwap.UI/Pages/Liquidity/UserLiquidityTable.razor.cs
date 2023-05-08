using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class UserLiquidityTable
{
    [Parameter]
    public IEnumerable<UserLiquidityData>? UserLiquidityData { get; set; }

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

    private void ExpandRow(int num)
	{
        ArgumentNullException.ThrowIfNull(UserLiquidityData);
        UserLiquidityData selectedData = UserLiquidityData.First(d => d.Number == num);
        selectedData.ShowDetails = !selectedData.ShowDetails;
	}
    
    private IEnumerable<UserLiquidityData>? _filteredData =>
        string.IsNullOrEmpty(_searchValue)
            ? UserLiquidityData
            : UserLiquidityData?.Where(
                d => d.TokenOneInfo.Token.Name.ToLower().Contains(_searchValue.ToLower()) || 
                     d.TokenTwoInfo.Token.Name.ToLower().Contains(_searchValue.ToLower())
            );
}
