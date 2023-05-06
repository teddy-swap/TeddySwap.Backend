using System.Text.Json;
using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class LiquidityTable
{
    [Parameter]
    public IEnumerable<TokenPairDetails>? TokenPairs { get; set; }

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
        ArgumentNullException.ThrowIfNull(TokenPairs);
        TokenPairDetails selectedPair = TokenPairs.First(t => t.Number == num);
        selectedPair.ShowDetails = !selectedPair.ShowDetails;
	}
    
    private IEnumerable<TokenPairDetails>? _filteredTokens =>
        string.IsNullOrEmpty(_searchValue)
            ? TokenPairs
            : TokenPairs?.Where(
                t => t.TokenPair.Tokens.Token1.Name.ToLower().Contains(_searchValue.ToLower()) || 
                     t.TokenPair.Tokens.Token2.Name.ToLower().Contains(_searchValue.ToLower())
            );
}
