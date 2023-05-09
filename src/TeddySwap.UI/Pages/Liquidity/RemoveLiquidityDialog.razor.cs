using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class RemoveLiquidityDialog
{
    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }

    [Parameter]
    public UserLiquidityData UserLiquidityData { get; set; } = new();

    private int removeAmount { get; set; }
    private string[] _labels = new string[] { "Min", "25%", "50%", "75%", "Max" };
    private Token TokenOne = new() { Name = "DJEDt", Logo = "../images/tokens/djed.png"  };
    private Token TokenTwo = new() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" };

}
