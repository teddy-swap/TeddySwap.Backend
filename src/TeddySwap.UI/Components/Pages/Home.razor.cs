using ApexCharts;
using Microsoft.AspNetCore.Components;
using TeddySwap.Data.Models;
using TeddySwap.Data.Models.Reducers;
using TeddySwap.Data.Services;
using TeddySwap.Data.Utils;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Components.Pages;

public partial class Home
{
    [Inject]
    protected CardanoDataService CardanoDataService { get; set; } = default!;

    [Inject]
    protected YieldFarmingDataService YieldFarmingDataService { get; set; } = default!;

    protected ulong CurrentBlockNumber => CardanoDataService.CurrentBlockNumber;
    protected IEnumerable<YieldRewardByAddress> Rewards { get; set; } = default!;
    protected IEnumerable<YieldFarmingDistribution> Distribution { get; set; } = default!;
    protected IEnumerable<YieldFarmingDistribution> ProjectedDistribution => Distribution is null ? [] : Distribution.Select(d =>
    {
        var month = YieldFarmingUtils.GetMonthFromSlot(d.Slot, YieldFarmingUtils.YF_START_SLOT);
        var dailyRewardAmount = YieldFarmingUtils.GetDailyRewardAmount(month);
        return new YieldFarmingDistribution
        {
            BlockNumber = d.BlockNumber,
            Amount = (ulong)(dailyRewardAmount * 1_000_000),
            Slot = d.Slot
        };
    });

    protected ApexPointSeries<YieldRewardByAddress>? yieldRewardSeries = default;
    protected ApexPointSeries<YieldFarmingDistribution>? distributionSeries = default;
    protected ApexPointSeries<YieldFarmingDistribution>? projectedDistributionSeries = default;

    private readonly ApexChartOptions<YieldRewardByAddress> YieldRewardOptions = new()
    {
        Chart = new Chart
        {
            Stacked = true,
            Background = "transparent"
        },
        PlotOptions = new PlotOptions
        {
            Bar = new PlotOptionsBar
            {
                DataLabels = new PlotOptionsBarDataLabels
                {
                    Total = new BarTotalDataLabels
                    {
                        Style = new BarDataLabelsStyle
                        {
                            FontWeight = "800"
                        }
                    }
                }
            }
        },
        Theme = new Theme
        {
            Mode = Mode.Dark,
            Palette = PaletteType.Palette1
        },
        Xaxis = new XAxis
        {
            Labels = new XAxisLabels
            {
                Formatter = "(value) => new Date(value).toLocaleDateString('en-US',{month: 'short',day: 'numeric'})"
            }
        },
        Markers = new Markers { Shape = ShapeEnum.Circle, Size = 5, FillOpacity = new Opacity(0.8d) },
        Yaxis = [
            new YAxis
            {
                Labels = new YAxisLabels
                {
                    Formatter = "(value) => value + ' $TEDY'"
                }
            }
        ],
    };

    private readonly ApexChartOptions<YieldFarmingDistribution> DistributionOptions = new()
    {
        Chart = new Chart
        {
            Stacked = true,
            StackType = StackType.Normal,
            Background = "transparent",
        },
        Theme = new Theme
        {
            Mode = Mode.Dark,
            Palette = PaletteType.Palette1
        },
        Markers = new Markers { Shape = ShapeEnum.Circle, Size = 5, FillOpacity = new Opacity(0.8d) },
        Xaxis = new XAxis
        {
            Labels = new XAxisLabels
            {
                Formatter = "(value) => value"
            }
        },
        Yaxis = [
            new YAxis
            {
                Labels = new YAxisLabels
                {
                    Formatter = "(value) => value + ' $TEDY'"
                }
            },
            new YAxis
            {
                Opposite = true,
                Labels= new YAxisLabels
                {
                    Formatter = "(value) => value + ' $TEDY'",
                    Show = false
                },
            }
        ],
    };

    protected override void OnInitialized()
    {
        CardanoDataService.Heartbeat += OnHeartbeat;
        base.OnInitialized();
    }

    private async void OnHeartbeat(object? sender, HeartbeatEventArgs e)
    {
        await RefreshAsync();
        
        if (yieldRewardSeries is not null)
            await yieldRewardSeries.Chart.UpdateSeriesAsync(true);
        
        if (distributionSeries is not null)
            await distributionSeries.Chart.UpdateSeriesAsync(true);
        
        if (projectedDistributionSeries is not null)
            await projectedDistributionSeries.Chart.UpdateSeriesAsync(true);

        await InvokeAsync(() => StateHasChanged());
    }

    protected async override Task OnInitializedAsync()
    {
        await RefreshAsync();
        await base.OnInitializedAsync();
    }

    protected async Task RefreshAsync()
    {
        var address = "addr1qxhwefhsv6xn2s4sn8a92f9m29lwj67aykn4plr9xal4r48del5pz2hf795j5wxzhzf405g377jmw7a92k9z2enhd6pqlal6jy";
        Rewards = await YieldFarmingDataService.YieldRewardByAddressAsync(address);
        Distribution = await YieldFarmingDataService.YieldRewardDistributionAsync();
        await InvokeAsync(StateHasChanged);
    }

    protected string PoolIdToPair(string poolId)
    {
        var pair = poolId.Split(".")[1].Split('_')[0..2];
        return $"{pair[0]}/{pair[1]}";
    }
}