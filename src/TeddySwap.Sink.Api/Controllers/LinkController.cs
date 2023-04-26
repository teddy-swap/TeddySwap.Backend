using Microsoft.AspNetCore.Mvc;
using TeddySwap.Common.Models.Request;
using TeddySwap.Common.Models.Response;
using TeddySwap.Sink.Api.Services;
using Asp.Versioning;
using TeddySwap.Common.Utils;
using System.Text.Json;
using TeddySwap.Common.Models;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Utilities;
using CardanoSharp.Wallet.Extensions.Models;

namespace TeddySwap.Sink.Api.Controllers;

[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]

public class LinkController : ControllerBase
{
    private readonly ILogger<LinkController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AddressVerificationService _addressVerificationService;

    public LinkController(
        ILogger<LinkController> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        AddressVerificationService addressVerificationService)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _addressVerificationService = addressVerificationService;
    }
    
    [HttpGet("{address}")]
    public async Task<ActionResult<string?>> GetMainnetAddressByTestnetAddressAsync([FromRoute] string address)
    {
        return await _addressVerificationService.GetMainnetAddressByTestnetAddressAsync(address);
    }

    [HttpPost]
    public async Task<IActionResult> LinkAddressAsync([FromBody] LinkAddressRequest linkAddressReq)
    {
        string verifyApiUrl = $"{(_configuration["VERIFYAPIURL"] ?? "http://localhost:8000")}/api/link";
        HttpClient httpClient = _httpClientFactory.CreateClient();
        HttpResponseMessage resp = await httpClient.PostAsJsonAsync(verifyApiUrl, linkAddressReq);
        resp.EnsureSuccessStatusCode();
        VerifyMessageResponse? verifyResponse = await resp.Content.ReadFromJsonAsync<VerifyMessageResponse>();

        if (verifyResponse is not null && verifyResponse.HasSigned && linkAddressReq.Payload is not null)
        {
            LinkAddressPayload? linkAddressPayload = JsonSerializer.Deserialize<LinkAddressPayload>(linkAddressReq.Payload.FromHex());

            foreach (string testnetAddress in linkAddressPayload?.TestnetAddresses!)
            {
                if (testnetAddress == linkAddressReq.Address)
                {
                    await _addressVerificationService.AddVerificationAsync(testnetAddress, linkAddressPayload.MainnetAddress!, JsonSerializer.Serialize(linkAddressReq).ToHex());
                }
            }

            return Ok();
        }
        else
        {
            return BadRequest(verifyResponse);
        }
    }
}