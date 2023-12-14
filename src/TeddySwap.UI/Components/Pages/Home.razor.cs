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

    public List<ChartSeries> Series = new()
    {
        new ChartSeries() { Name = "United States", Data = [40, 20, 25, 27, 46, 60, 48, 80, 15] },
        new ChartSeries() { Name = "Germany", Data = [19, 24, 35, 13, 28, 15, 13, 16, 31] },
        new ChartSeries() { Name = "Sweden", Data = [8, 6, 11, 13, 4, 16, 10, 16, 18] },
    };
    public string[] XAxisLabels = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep"];
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