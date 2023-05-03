using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Models;
using MudBlazor;

namespace TeddySwap.UI.Shared;

public partial class TokenSelectionDialog
{
    [CascadingParameter]
    MudDialogInstance? MudDialog { get; set; }

    [Parameter]
    public IEnumerable<Token> Tokens { get; set; } = new List<Token>();

    [Parameter]
    public Action<Token> OnSelectedTokenClicked { get; set; } = default!;
    
    private string _searchValue { get; set; } = string.Empty;

    private IEnumerable<Token> _filteredTokens =>
        string.IsNullOrEmpty(_searchValue)
            ? Tokens
            : Tokens.Where(t => t.Name.ToLower().Contains(_searchValue.ToLower())).ToList();
}
