using Microsoft.AspNetCore.Components;
using MudBlazor;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Components.Pages;

public partial class Home
{
    [Inject]
    protected CardanoDataService CardanoDataService { get; set; } = default!;
    protected ulong CurrentBlockNumber => CardanoDataService.CurrentBlockNumber;


    #region Example
    private int Index = -1; //default value cannot be 0 -> first selectedindex is 0.

    public List<ChartSeries> Series =
    [
        new ChartSeries() { Name = "Base Reward", Data = [40, 20, 25, 27, 46, 60, 48, 80, 15] },
        new ChartSeries() { Name = "Bonus Reward", Data = [19, 24, 35, 13, 28, 15, 13, 16, 31] },
        new ChartSeries() { Name = "Total Reward", Data = [8, 6, 11, 13, 4, 16, 10, 16, 18] },
    ];
    public string[] XAxisLabels = ["Dec 1", "Dec 2", "Dec 3", "Dec 4", "Dec 5", "Dec 6", "Dec 7", "Dec 8", "Dec 9"];
    #endregion


    protected override void OnInitialized()
    {
        CardanoDataService.Heartbeat += OnHeartbeat;
        base.OnInitialized();
    }

    private void OnHeartbeat(object? sender, HeartbeatEventArgs e)
    {
        InvokeAsync(() => StateHasChanged());
    }
}