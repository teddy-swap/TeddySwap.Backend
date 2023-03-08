using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TeddySwap.Common.Models.Request;
using TeddySwap.Sink.Api.Services;
using TeddySwap.Sink.Data;

namespace TeddySwap.Sink.Api.Controllers;

[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class OutputsController : ControllerBase
{
    private readonly ILogger<OutputsController> _logger;
    private readonly CardanoDbSyncContext _dbContext;
    private readonly OutputService _outputService;

    public OutputsController(
        ILogger<OutputsController> logger,
        CardanoDbSyncContext dbSyncContext,
        OutputService outputService)
    {
        _dbContext = dbSyncContext;
        _outputService = outputService;
        _logger = logger;
    }

    [HttpGet("pkh/{pkh}")]
    public async Task<IActionResult> GetOutputsByPkhAsync(string pkh, [FromQuery] PaginatedRequest request)
    {
        if (request.Offset < 0 || request.Limit > 100 || string.IsNullOrEmpty(pkh)) return BadRequest();

        var res = await _outputService.GetUtxosByPaymentKeyHashAsync(request.Offset, request.Limit, pkh);

        return Ok(res);
    }

    [HttpGet("address/{address}")]
    public async Task<IActionResult> GetOutputsByAddressAsync(string address, [FromQuery] PaginatedRequest request)
    {
        if (request.Offset < 0 || request.Limit > 100 || string.IsNullOrEmpty(address)) return BadRequest();

        var res = await _outputService.GetUtxosByAddressAsync(request.Offset, request.Limit, address);

        return Ok(res);
    }
}