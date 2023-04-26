using Microsoft.AspNetCore.Mvc;
using TeddySwap.Common.Models.Request;
using TeddySwap.Common.Models.Response;
using TeddySwap.Sink.Api.Services;
using Asp.Versioning;

namespace TeddySwap.Sink.Api.Controllers;

[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly ILogger<AssetsController> _logger;
    private readonly AssetService _assetService;

    public AssetsController(
        ILogger<AssetsController> logger,
        AssetService assetService)
    {
        _logger = logger;
        _assetService = assetService;
    }

    [HttpGet("policy/{policyId}/address/{address}")]
    public async Task<ActionResult<PaginatedAssetResponse>> GetNftOwnerAsync([FromRoute] string policyId, [FromRoute] string address, [FromQuery] PaginatedRequest request)
    {
        var res = await _assetService.GetNftOwnerAsync(request.Offset, request.Limit, address, policyId);
        return Ok(res);
    }

    [HttpGet("policy/{policyId}/stakeAddress/{stakeAddress}")]
    public async Task<ActionResult<PaginatedAssetResponse>> GetNftOwnerByStakeAddressAsync([FromRoute] string policyId, [FromRoute] string stakeAddress, [FromQuery] PaginatedRequest request)
    {
        var res = await _assetService.GetNftOwnerByStakeAddressAsync(request.Offset, request.Limit, stakeAddress, policyId);
        return Ok(res);
    }

    [HttpPost("policy/{policyId}")]
    public async Task<ActionResult<AssetMetadataResponse>> GetNftOwnersAsync([FromQuery] PaginatedRequest request, [FromBody] List<string> addresses, [FromRoute] string policyId)
    {
        if (addresses is null || addresses.Count < 1) return BadRequest();

        var res = await _assetService.GetNftOwnersAsync(request.Offset, request.Limit, addresses, policyId);

        return Ok(res);
    }

    [HttpGet("metadata/policy/{policyId}/name/{name}")]
    public async Task<ActionResult<AssetMetadataResponse>> GetAssetMetadataAsync([FromRoute] string policyId, [FromRoute] string name)
    {
        if (string.IsNullOrEmpty(policyId) || string.IsNullOrEmpty(name)) return BadRequest();

        var res = await _assetService.GetNftMetadataAsync(policyId, name);

        return Ok(res);
    }

    [HttpPost("metadata")]
    public async Task<ActionResult<AssetMetadataResponse>> GetAssetMetadataAsync([FromBody] List<string> assets)
    {
        if (assets is null || assets.Count < 1) return BadRequest();

        var res = await _assetService.GetNftMetadataAsync(assets);

        return Ok(res);
    }
}