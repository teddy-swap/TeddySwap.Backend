using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Shared;

public partial class ConfirmationDialog
{
    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }
    
    [Parameter]
    public string ButtonTitle { get; set; } = string.Empty;

    [Parameter]
    public IEnumerable<TokenAsset>? TokenAssets { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private Token Token = new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" };
}
