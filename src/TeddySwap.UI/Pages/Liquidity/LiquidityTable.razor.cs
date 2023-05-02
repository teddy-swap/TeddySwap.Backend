using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class LiquidityTable
{
    [Parameter]
    public IEnumerable<TokenPairDetails>? TokenPairs { get; set; }

    private string _searchValue { get; set; } = string.Empty;

    private void ExpandRow(int num)
	{
        ArgumentNullException.ThrowIfNull(TokenPairs);
        TokenPairDetails selectedPair = TokenPairs.First(t => t.Number == num);
        selectedPair.ShowDetails = !selectedPair.ShowDetails;
	}
    // private IEnumerable<TokenPairDetails> _filteredTokens =>
    //     string.IsNullOrEmpty(_searchValue)
    //         ? TokenPairs
    //         : TokenPairs.Where(t => t.Name.ToLower().Contains(_searchValue.ToLower())).ToList();
}
