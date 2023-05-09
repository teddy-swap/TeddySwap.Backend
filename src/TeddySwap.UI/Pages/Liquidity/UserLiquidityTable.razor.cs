using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class UserLiquidityTable
{
    [Inject]
    IDialogService? DialogService { get; set; }

    [Parameter]
    public IEnumerable<UserLiquidityData>? UserLiquidityData { get; set; }

    public IEnumerable<Token>? _tokens { get; set; }
    private string? _searchValue { get; set; }
    private Token? _currentlySelectedToken { get; set; }

    protected override void OnInitialized()
    {
        string tokensJson = File.ReadAllText("./wwwroot/tokens.json");
        ArgumentException.ThrowIfNullOrEmpty(tokensJson);
        _tokens = JsonSerializer.Deserialize<IEnumerable<Token>>(tokensJson);
    }

    private void HandleSelectedToken(Token token)
    {
        _searchValue = token.Name;
        _currentlySelectedToken = token;
        StateHasChanged();
    }

    private void ExpandRow(int num)
	{
        UserLiquidityData rowData = GetRowData(num);
        rowData.ShowDetails = !rowData.ShowDetails;
	}

    private void OpenPoolOverviewDialog(int num)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters();
        UserLiquidityData rowData = GetRowData(num);
        parameters.Add("UserLiquidityData", rowData);
        DialogService?.Show<PoolOverviewDialog>("Pool Overview", parameters, options);
    }

    private UserLiquidityData GetRowData(int num)
    {
        ArgumentNullException.ThrowIfNull(UserLiquidityData);
        return UserLiquidityData.First(d => d.Number == num);
    }
    
    private IEnumerable<UserLiquidityData>? _filteredData =>
        string.IsNullOrEmpty(_searchValue)
            ? UserLiquidityData
            : UserLiquidityData?.Where(
                d => d.TokenOneInfo.Token.Name.ToLower().Contains(_searchValue.ToLower()) || 
                     d.TokenTwoInfo.Token.Name.ToLower().Contains(_searchValue.ToLower())
            );
}
