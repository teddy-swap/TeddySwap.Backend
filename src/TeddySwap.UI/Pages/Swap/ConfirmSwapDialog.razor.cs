using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Services;
using MudBlazor;

namespace TeddySwap.UI.Pages.Swap;

public partial class ConfirmSwapDialog
{
    [Inject]
    public AppStateService? AppStateService { get; set; }

    [Inject]
    public IconsService? IconsService { get; set; }

    [Inject]
    IDialogService? DialogService { get; set; }

    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }

    private void OpenWaitingConfirmationDialog()
    {
        ArgumentNullException.ThrowIfNull(DialogService);
        var options = new DialogOptions { CloseOnEscapeKey = true };
        DialogService.Show<WaitingConfirmationDialog>("", options);
    }
}
