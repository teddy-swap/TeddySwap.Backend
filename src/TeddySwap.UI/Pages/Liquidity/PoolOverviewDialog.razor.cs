using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Models;
using TeddySwap.UI.Services;
using MudBlazor;


namespace TeddySwap.UI.Pages.Liquidity;

public partial class PoolOverviewDialog
{   
    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }

    [Inject]
    public AppStateService? AppStateService { get; set; }

    private Token _tokenOne = new() { Logo = "../images/tokens/djed.png", Name = "DJEDt" };
    private Token _tokenTwo = new() { Logo = "../images/tokens/token-ada.svg", Name = "ADA" };
}
