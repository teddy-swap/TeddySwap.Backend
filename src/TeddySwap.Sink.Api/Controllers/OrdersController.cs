using Microsoft.AspNetCore.Mvc;
using TeddySwap.Common.Models.Request;
using TeddySwap.Sink.Api.Services;

namespace TeddySwap.Sink.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestLiquidityPoolsAsync([FromQuery] PaginatedRequest request)
    {
        int limit = request.Limit <= 0 ? 100 : request.Limit;
        int offset = request.Offset < 0 ? 0 : request.Offset;

        var res = await _orderService.GetLatestSwapOrdersAsync(offset, limit);

        return Ok(res);
    }

    [HttpGet("executed/address/{address}")]
    public async Task<IActionResult> GetLatestExecutedSwapOrdersByAddressAsync([FromRoute] string address, [FromQuery] PaginatedRequest request)
    {
        int limit = request.Limit <= 0 ? 100 : request.Limit;
        int offset = request.Offset < 0 ? 0 : request.Offset;

        var res = await _orderService.GetLatestExecutedSwapOrdersByAddressAsync(address, offset, limit);

        return Ok(res);
    }
}