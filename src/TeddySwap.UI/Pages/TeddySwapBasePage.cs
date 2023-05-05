using Microsoft.AspNetCore.Components;
using TeddySwap.Common.Services;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Pages;

public class TeddySwapBasePage : ComponentBase, IAsyncDisposable
{
    [Inject]
    protected HeartBeatService? HeartBeatService { get; set; }


    protected async override Task OnInitializedAsync()
    {
        if (HeartBeatService is not null)
        {
            HeartBeatService.Hearbeat += OnHeartBeatEvent;
        }
    }

    protected virtual void OnHeartBeatEvent(object? sender, EventArgs e)
    {
    }

    public async ValueTask DisposeAsync()
    {
        if (HeartBeatService is not null)
        {
            HeartBeatService.Hearbeat -= OnHeartBeatEvent;
        }
    }
}