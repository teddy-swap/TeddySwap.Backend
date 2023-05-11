using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace TeddySwap.UI.Shared;

public partial class TransactionSubmittedDialog
{
    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }

    [Parameter]
    public string TransactionLink { get; set; } = string.Empty;
}
