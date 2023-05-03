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

    [Parameter, EditorRequired]
    public IEnumerable<Token> Tokens { get; set; } = new List<Token>();

    [Parameter, EditorRequired]
    public Token CurrentlySelectedToken { get; set; } = new();

    public EventCallback<Token> OnSelectedTokenClicked { get; set; }

    [Parameter, EditorRequired]
    public string Id { get; set; } = string.Empty;

    [Parameter]
    public bool Disabled { get; set; } = false;

    private void OpenTokenSelectionDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters();
        parameters.Add("Tokens", Tokens);
        parameters.Add("OnSelectedTokenClicked", (Token token) => HandleSelectedToken(token));
        DialogService?.Show<TokenSelectionDialog>("Select Token", parameters, options);
    }

    private void HandleSelectedToken(Token token)
    {
        if (Id == "1") AppStateService.FromCurrentlySelectedToken = token;
        if (Id == "2") AppStateService.ToCurrentlySelectedToken = token;
    }
}
