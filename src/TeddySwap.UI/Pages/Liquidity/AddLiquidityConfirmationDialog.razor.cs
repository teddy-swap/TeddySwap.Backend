using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class AddLiquidityConfirmationDialog
{
    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }

    private Token _testToken1 = new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" };

    private Token _testToken2 = new Token() { Name = "DJEDt", Logo = "../images/tokens/djed.png" };
}
