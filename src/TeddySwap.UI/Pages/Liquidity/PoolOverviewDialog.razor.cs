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

    [Inject]
    IDialogService? DialogService { get; set; }

    [Parameter, EditorRequired]
    public UserLiquidityData UserLiquidityData { get; set; } = new();

    private void OpenRemoveLiquidityDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters();
        parameters.Add("UserLiquidityData", UserLiquidityData);
        DialogService?.Show<RemoveLiquidityDialog>("Remove Liquidity", parameters, options);
    }
}
