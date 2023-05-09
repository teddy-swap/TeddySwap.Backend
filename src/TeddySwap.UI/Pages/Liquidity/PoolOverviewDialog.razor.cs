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

    [Parameter, EditorRequired]
    public UserLiquidityData UserLiquidityData { get; set; } = new();
}
