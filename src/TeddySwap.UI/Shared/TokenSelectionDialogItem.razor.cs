using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Models;
using MudBlazor;

namespace TeddySwap.UI.Shared;

public partial class TokenSelectionDialogItem
{
    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public Token Token { get; set; } = default!;

    [Parameter]
    public EventCallback<Token> OnSelectedTokenClicked { get; set; }

    private async Task HandleItemClicked()
    {
        await OnSelectedTokenClicked.InvokeAsync(Token);
        MudDialog.Close();
    }
}
