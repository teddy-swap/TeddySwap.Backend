using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.UI.Models;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Shared;

public partial class TokenChip
{
    [Inject]
    IDialogService? DialogService { get; set; }

    [Inject]
    protected new AppStateService? AppStateService { get; set; }

    [Parameter]
    public IEnumerable<Token> Tokens { get; set; } = new List<Token>();

    [Parameter, EditorRequired]
    public Token? CurrentlySelectedToken { get; set; }

    [Parameter]
    public Action<Token>? HandleSelectedToken { get; set; }

    [Parameter]
    public bool Disabled { get; set; } = false;

    private void OpenTokenSelectionDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters();
        parameters.Add("Tokens", Tokens);
        parameters.Add("OnSelectedTokenClicked", (Token token) => HandleSelectedToken?.Invoke(token));
        DialogService?.Show<TokenSelectionDialog>("Select Token", parameters, options);
    }
}