using Microsoft.AspNetCore.Mvc;
using TeddySwap.Common.Models.Request;
using TeddySwap.Common.Models.Response;
using TeddySwap.Sink.Api.Services;
using Asp.Versioning;
using TeddySwap.Common.Enums;

namespace TeddySwap.Sink.Api.Controllers;

[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly ILogger<LeaderboardController> _logger;
    private readonly LeaderboardService _leaderboardService;

    public LeaderboardController(
        ILogger<LeaderboardController> logger,
        LeaderboardService leaderboardService)
    {
        _logger = logger;
        _leaderboardService = leaderboardService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedLeaderBoardResponse>> GetLeaderboardAsync([FromQuery] PaginatedRequest request)
    {
        if (request.Offset < 0 || request.Limit > 100) return BadRequest();

        var res = await _leaderboardService.GetLeaderboardAsync(request.Offset, request.Limit, LeaderBoardType.All);

        return Ok(res);
    }

    [HttpGet("address/{address}")]
    public async Task<ActionResult<LeaderBoardResponse>> GetLeaderboardAddressAsync(string? address)
    {
        if (string.IsNullOrEmpty(address)) return BadRequest();

        var res = await _leaderboardService.GetLeaderboardAddressAsync(address, LeaderBoardType.All);

        if (res is null) return NotFound();

        return Ok(res);
    }

    [HttpGet("users")]
    public async Task<ActionResult<PaginatedLeaderBoardResponse>> GetUserLeaderboardAsync([FromQuery] PaginatedRequest request)
    {
        if (request.Offset < 0 || request.Limit > 100) return BadRequest();

        var res = await _leaderboardService.GetLeaderboardAsync(request.Offset, request.Limit, LeaderBoardType.Users);

        return Ok(res);
    }

    [HttpGet("users/address/{address}")]
    public async Task<ActionResult<LeaderBoardResponse>> GetUserLeaderboardAddressAsync(string? address)
    {
        if (string.IsNullOrEmpty(address)) return BadRequest();

        var res = await _leaderboardService.GetLeaderboardAddressAsync(address, LeaderBoardType.Users);

        if (res is null) return NotFound();

        return Ok(res);
    }

    [HttpGet("badgers")]
    public async Task<ActionResult<PaginatedLeaderBoardResponse>> GetBatcherLeaderboardAsync([FromQuery] PaginatedRequest request)
    {
        if (request.Offset < 0 || request.Limit > 100) return BadRequest();

        var res = await _leaderboardService.GetLeaderboardAsync(request.Offset, request.Limit, LeaderBoardType.Badgers);

        return Ok(res);
    }

    [HttpGet("badgers/address/{address}")]
    public async Task<ActionResult<LeaderBoardResponse>> GetBatcherLeaderboardAddressAsync(string? address)
    {
        if (string.IsNullOrEmpty(address)) return BadRequest();

        var res = await _leaderboardService.GetLeaderboardAddressAsync(address, LeaderBoardType.Badgers);

        if (res is null) return NotFound();

        return Ok(res);
    }

}