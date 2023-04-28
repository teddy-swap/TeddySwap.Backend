using Microsoft.AspNetCore.Mvc;
using TeddySwap.Common.Models.Request;
using TeddySwap.Sink.Api.Services;

namespace TeddySwap.Sink.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LiquidityPoolsController : ControllerBase
{
    private readonly OrderService _orderService;
    public LiquidityPoolsController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestLiquidityPoolsAsync([FromQuery] PaginatedRequest request)
    {
        int limit = request.Limit <= 0 ? 100 : request.Limit;
        int offset = request.Offset < 0 ? 0 : request.Offset;

        var res = await _orderService.GetLatestLiquidityPoolsAsync(offset, limit);

        return Ok(res);
    }
}