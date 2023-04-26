using System.Text.Json;
using TeddySwap.Common.Enums;
using TeddySwap.Common.Models;
using TeddySwap.Common.Models.Request;
using TeddySwap.Common.Models.Response;

namespace TeddySwap.UI.Services;

public class SinkService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ConfigService _configService;

    public SinkService(IHttpClientFactory clientFactory, ConfigService configService)
    {
        _clientFactory = clientFactory;
        _configService = configService;
    }

    public async Task<PaginatedLeaderBoardResponse> GetLeaderboardAsync(LeaderBoardType leaderboardType = LeaderBoardType.Users, int offset = 0, int limit = 10, string? address = null)
    {
        using HttpClient httpClient = _clientFactory.CreateClient();
        string leaderboardTypeString = leaderboardType == LeaderBoardType.Users ? "users" : "badgers";
        if (address is not null && address != string.Empty)
        {
            PaginatedLeaderBoardResponse? response = await httpClient
                .GetFromJsonAsync<PaginatedLeaderBoardResponse>(
                    $"{_configService.SinkApiUrl}/api/v1/leaderboard/{leaderboardTypeString}/address/{address}"
                );
            return response ?? new PaginatedLeaderBoardResponse { Result = new List<LeaderBoardResponse>() { new LeaderBoardResponse() } };
        }
        else
        {
            PaginatedLeaderBoardResponse? response =
                await httpClient
                    .GetFromJsonAsync<PaginatedLeaderBoardResponse>(
                        $"{_configService.SinkApiUrl}/api/v1/leaderboard/{leaderboardTypeString}?offset={offset}&limit={limit}"
                    );
            if (response is null) throw new HttpRequestException("Bad response from GetLeaderboardAsync.");
            return response;
        }
    }

    public async Task<PaginatedLeaderBoardResponse> GetRewardFromAddressesAsync(string[] addresses)
    {
        using HttpClient httpClient = _clientFactory.CreateClient();
        HttpResponseMessage resp = await httpClient
                .PostAsJsonAsync($"{_configService.SinkApiUrl}/api/v1/leaderboard/users/addresses", new { addresses });
        return await resp.Content.ReadFromJsonAsync<PaginatedLeaderBoardResponse>() ?? throw new HttpRequestException("Bad response from GetRewardFromAddressesAsync.");
    }

    public async Task<string> GetMainnetAddressFromTestnetAddressAsync(string testnetAddress)
    {
        using HttpClient httpClient = _clientFactory.CreateClient();
        return await httpClient.GetStringAsync($"{_configService.SinkApiUrl}/api/v1/link/{testnetAddress}");
    }

    public async Task LinkMainnetAddressAsync(string signerAddress, string payload, CardanoSignedMessage signedMessage)
    {
        using HttpClient httpClient = _clientFactory.CreateClient();
        HttpResponseMessage resp = await httpClient.PostAsJsonAsync($"{_configService.SinkApiUrl}/api/v1/link", new LinkAddressRequest()
        {
            Address = signerAddress,
            Payload = payload,
            SignedMessage = signedMessage
        });
        Console.WriteLine(await resp.Content.ReadAsStringAsync());
    }

    public async Task<int> GetNftCountByStakeAddressPolicyAsync(string address, string policyId)
    {
        PaginatedAssetResponse? resp = await GetPolicyAssetsByStakeAddressAsync(address, policyId);
        return resp?.TotalCount ?? 0;
    }

    public async Task<PaginatedAssetResponse?> GetPolicyAssetsByStakeAddressAsync(string address, string policyId, int limit = 10, int offset = 0)
    {
        using HttpClient httpClient = _clientFactory.CreateClient();
        return await httpClient.GetFromJsonAsync<PaginatedAssetResponse>($"{_configService.SinkApiUrl}/api/v1/Assets/policy/{policyId}/stakeaddress/{address}?limit={limit}&offset={offset}");
    }

    public async Task<double> GetFisoRewardByStakeAddressAsync(string stakeAddress)
    {
        using HttpClient httpClient = _clientFactory.CreateClient();
        FisoRewardBreakdownResponse? resp = await httpClient.GetFromJsonAsync<FisoRewardBreakdownResponse>($"{_configService.SinkApiUrl}/api/v1/FisoRewards/address/{stakeAddress}");
        return resp?.TotalBaseReward ?? 0d;
    }
}