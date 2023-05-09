using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Models;
using MudBlazor;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class RemoveLiquidityConfirmationDialog
{
    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }

    [Parameter]
    public Token TokenOne { get; set; } = new();

    [Parameter]
    public Token TokenTwo { get; set; } = new();
}
