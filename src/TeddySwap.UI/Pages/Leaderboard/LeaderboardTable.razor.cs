using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.Common.Enums;
using TeddySwap.Common.Models.Response;
using TeddySwap.UI.Models;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Pages.Leaderboard;

public partial class LeaderboardTable
{
    [Inject]
    protected SinkService? SinkService { get; set; }
    protected IEnumerable<LeaderBoardItem>? LeaderBoardItems { get; set; }
    protected MudTable<LeaderBoardItem>? LeaderBoardTable { get; set; }
    protected string SearchQuery { get; set; } = string.Empty;
    protected PaginatedLeaderBoardResponse LeaderBoardStats { get; set; } = new();

    [Parameter]
    public LeaderBoardType LeaderBoardType { get; set; } = LeaderBoardType.Users;

    protected override async Task OnInitializedAsync()
    {
        await RefreshDataAsync();
        await base.OnInitializedAsync();
    }

    public async Task RefreshDataAsync()
    {
        await InvokeAsync(async () =>
        {
            if (LeaderBoardTable is not null)
                await LeaderBoardTable.ReloadServerData();
            if (SinkService is not null)
                LeaderBoardStats = await SinkService.GetLeaderboardAsync(LeaderBoardType, 0, 0);
        });
    }

    protected async Task<TableData<LeaderBoardItem>> LeaderboardServerData(TableState ts)
    {
        TableData<LeaderBoardItem> tableData = new TableData<LeaderBoardItem>();
        if (SinkService is not null)
        {
            try
            {
                PaginatedLeaderBoardResponse resp = await SinkService.GetLeaderboardAsync(LeaderBoardType, ts.Page * ts.PageSize, ts.PageSize, SearchQuery);
                IEnumerable<LeaderBoardItem>? result = resp.Result.Select(lbr => LeaderBoardItem.FromResponse(lbr)).Where(lbr => lbr is not null) as IEnumerable<LeaderBoardItem>;
                if (result is not null)
                {
                    tableData.Items = result;
                    tableData.TotalItems = resp.TotalCount;
                }
                else
                {
                    tableData.Items = new List<LeaderBoardItem>();
                    tableData.TotalItems = 0;
                }
            }
            catch
            {
                // @TODO: Push error to analytics
                tableData.Items = new List<LeaderBoardItem>();
                tableData.TotalItems = 0;
            }
        }
        return tableData;
    }

    protected async Task OnSearch(string value)
    {
        SearchQuery = value;
        await RefreshDataAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
    }

    protected void ToggleRowExpand(LeaderBoardItem item)
    {
        item.IsExpanded = !item.IsExpanded;
    }

}