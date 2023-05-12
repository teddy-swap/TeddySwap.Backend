using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Shared;

public partial class TokenChip
{
    [Inject]
    IDialogService? DialogService { get; set; }

    [Parameter]
    public IEnumerable<Token> Tokens { get; set; } = new List<Token>();

    [Parameter, EditorRequired]
    public Token? CurrentlySelectedToken { get; set; }

    [Parameter]
    public EventCallback<Token> CurrentlySelectedTokenChanged { get; set; }

    [Parameter]
    public Action<Token>? OnSelectedToken { get; set; }

    [Parameter]
    public bool Disabled { get; set; } = false;

    private async Task OnCurrentySelectedTokenChanged(Token token)
    {   
        OnSelectedToken?.Invoke(token);
        await CurrentlySelectedTokenChanged.InvokeAsync(token);
    }
    
    private void OpenTokenSelectionDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters();
        parameters.Add("Tokens", Tokens);
        parameters.Add("OnSelectedTokenClicked", (Token token) => OnSelectedToken?.Invoke(token));
        DialogService?.Show<TokenSelectionDialog>("Select Token", parameters, options);
    }
}
