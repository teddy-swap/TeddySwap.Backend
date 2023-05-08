using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.UI.Models;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class AddLiquidityConfirmationDialog
{
    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }

    [Inject]
    public AppStateService? AppStateService { get; set; }

    private bool _isConfirmed { get; set; } = false;
    private void HandleConfirmationBtnClicked() => _isConfirmed = true;
}
