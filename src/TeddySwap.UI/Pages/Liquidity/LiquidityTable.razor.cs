using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class LiquidityTable
{
    [Parameter]
    public IEnumerable<TokenPairDetails>? TokenPairs { get; set; }

    private string _search { get; set; } = string.Empty;

    private void ExpandRow(int num)
	{
        ArgumentNullException.ThrowIfNull(TokenPairs);
        TokenPairDetails selectedPair = TokenPairs.First(t => t.Number == num);
        selectedPair.ShowDetails = !selectedPair.ShowDetails;
	}
}
