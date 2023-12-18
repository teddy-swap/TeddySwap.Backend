using ApexCharts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.JSInterop;
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

    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    protected CacheService CacheService { get; set; } = default!;

    [Inject]
    protected ILogger<Home> Logger { get; set; } = default!;

    [Parameter]
    public string? Address { get; set; }

    protected decimal UnclaimedRewards { get; set; }
    protected bool IsLoading { get; set; }
    protected ulong CurrentBlockNumber => CardanoDataService.CurrentBlockNumber;
    protected IEnumerable<YieldRewardByAddress> Rewards { get; set; } = [];
    protected IEnumerable<YieldRewardByAddress> ClaimedRewards => Rewards is null ? [] : Rewards.Where(r => r.IsClaimed);
    protected List<YieldRewardByAddress> PaginatedRewards { get; set; } = [];
    protected IEnumerable<YieldFarmingDistribution> Distribution { get; set; } = [];
    protected IEnumerable<YieldFarmingDistribution> ClaimedDistribution { get; set; } = [];
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
    protected ApexPointSeries<YieldRewardByAddress>? claimedYieldRewardSeries = default;
    protected ApexPointSeries<YieldFarmingDistribution>? distributionSeries = default;
    protected ApexPointSeries<YieldFarmingDistribution>? distributionWithBonusSeries = default;
    protected ApexPointSeries<YieldFarmingDistribution>? projectedDistributionSeries = default;
    protected ApexPointSeries<YieldFarmingDistribution>? claimedDistributionSeries = default;

    private readonly ApexChartOptions<YieldRewardByAddress> YieldRewardOptions = new()
    {
        Chart = new Chart
        {
            Stacked = false,
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
            Stacked = false,
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

        await InvokeAsync(() => StateHasChanged());
    }

    protected async override Task OnInitializedAsync()
    {
        await RefreshAsync();
        await base.OnInitializedAsync();
    }

    protected async Task RefreshAsync()
    {
        try
        {
            if (Address is null) return;

            Rewards = await CacheService.GetOrCreateAsync($"YieldRewardByAddressSinceDaysAgoAsync_{Address}_30",
                async entry => await YieldFarmingDataService.YieldRewardByAddressSinceDaysAgoAsync(Address, 30) ?? []
            ) ?? [];

            Distribution = await CacheService.GetOrCreateAsync($"YieldRewardDistributionSinceDaysAgoAsync_30",
                async entry => await YieldFarmingDataService.YieldRewardDistributionSinceDaysAgoAsync(30) ?? []
            ) ?? [];

            UnclaimedRewards = await CacheService.GetOrCreateAsync($"TotalUnclaimedRewardsAsync_{Address}",
                async entry => await YieldFarmingDataService.TotalUnclaimedRewardsAsync(Address)
            ) / (decimal)1000000;

            ClaimedDistribution = await CacheService.GetOrCreateAsync($"ClaimedYieldRewardDistributionSinceDaysAgoAsync_30",
                async entry => await YieldFarmingDataService.ClaimedYieldRewardDistributionSinceDaysAgoAsync(30) ?? []
            ) ?? [];

            if (PaginatedRewards.Count > 0)
                PaginatedRewards = [.. await YieldFarmingDataService.YieldRewardByAddressAsync(Address, PaginatedRewards.First().Slot), .. PaginatedRewards];

            if (yieldRewardSeries is not null)
                await yieldRewardSeries.Chart.UpdateSeriesAsync(true);

            if (distributionSeries is not null)
                await distributionSeries.Chart.UpdateSeriesAsync(true);

            if (projectedDistributionSeries is not null)
                await projectedDistributionSeries.Chart.UpdateSeriesAsync(true);

            if (distributionWithBonusSeries is not null)
                await distributionWithBonusSeries.Chart.UpdateSeriesAsync(true);

            if (claimedDistributionSeries is not null)
                await claimedDistributionSeries.Chart.UpdateSeriesAsync(true);

            if (claimedYieldRewardSeries is not null)
                await claimedYieldRewardSeries.Chart.UpdateSeriesAsync(true);

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while refreshing data");
        }

    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadMoreRewardsAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected async Task LoadMoreRewardsAsync()
    {
        if (Address is null) return;

        IsLoading = true;
        await InvokeAsync(StateHasChanged);

        PaginatedRewards.AddRange(await YieldFarmingDataService.YieldRewardByAddressAsync(Address, 10, PaginatedRewards.Count));

        IsLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    protected string PoolIdToPair(string poolId)
    {
        var pair = poolId.Split(".")[1].Split('_')[0..2];
        return $"{pair[0]}/{pair[1]}";
    }

    protected async void OnHarvestClicked()
    {
        await JSRuntime.InvokeVoidAsync("window.harvest");
    }
}