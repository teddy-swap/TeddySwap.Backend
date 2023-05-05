using System.Text.Json;
using TeddySwap.Common.Services;
using TeddySwap.UI.Models;
using TeddySwap.UI.Services;

namespace TeddySwap.UI.Workers;

public class HeartBeatWorker : BackgroundService
{
    private readonly ILogger<HeartBeatWorker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ConfigService _configService;
    private readonly HeartBeatService _heartBeatService;

    public HeartBeatWorker(
        ILogger<HeartBeatWorker> logger,
        IHttpClientFactory httpClientFactory,
        ConfigService configService,
        HeartBeatService heartbeatService)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configService = configService;
        _heartBeatService = heartbeatService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient();
                BlockInfoResponse? blockInfoResponse = await httpClient.GetFromJsonAsync<BlockInfoResponse>($"{_configService.ExplorerApiUrl}/cardano/v1/blocks/bestBlock");
                ulong latestBlockNo = blockInfoResponse?.BlockNo ?? _heartBeatService.LatestBlockNo;
                ulong latestSlotNo = blockInfoResponse?.SlotNo ?? _heartBeatService.LatestSlotNo;
                if (latestBlockNo > _heartBeatService.LatestBlockNo)
                {
                    _heartBeatService.LatestBlockNo = blockInfoResponse?.BlockNo ?? _heartBeatService.LatestBlockNo;
                    _heartBeatService.LatestSlotNo = blockInfoResponse?.SlotNo ?? _heartBeatService.LatestSlotNo;
                    _heartBeatService.TriggerHeartBeat();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Blockinfo Request failed.");
            }
            await Task.Delay(20_000);
        }
    }
}