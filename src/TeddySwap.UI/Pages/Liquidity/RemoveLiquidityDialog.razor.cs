using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class RemoveLiquidityDialog
{
    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }

    [Inject]
    IDialogService? DialogService { get; set; }

    [Parameter]
    public UserLiquidityData UserLiquidityData { get; set; } = new();

    private int removeAmount { get; set; }

    private void OpenRemoveConfirmationDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters();
        parameters.Add("TokenOne", UserLiquidityData?.TokenOneInfo?.Token);
        parameters.Add("TokenTwo", UserLiquidityData?.TokenTwoInfo?.Token);
        DialogService?.Show<RemoveLiquidityConfirmationDialog>("Remove Liquidity", parameters, options);
    }
}
