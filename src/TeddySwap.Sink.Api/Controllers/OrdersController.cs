using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TeddySwap.Common.Models.Request;
using TeddySwap.Sink.Api.Services;
using TeddySwap.Sink.Data;

namespace TeddySwap.Sink.Api.Controllers;

[ApiVersion(1.0)]
[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly OrderService _orderService;

    public OrdersController(ILogger<OrdersController> logger, OrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersAsync([FromQuery] PaginatedRequest request)
    {
        if (request.Offset < 0) return BadRequest();

        var res = await _orderService.GetOrdersAsync(request.Offset, request.Limit);

        return Ok(res);
    }

    [HttpGet("median")]
    public async Task<IActionResult> GetOrderMedianAsync()
    {

        var res = await _orderService.GetMedianTransactionCountAsync();

        return Ok(res);
    }

    [HttpGet("address/{address}")]
    public async Task<IActionResult> GetAddressOrdersAsync(string address)
    {

        var res = await _orderService.GetAddressOrdersAsync(address);

        return Ok(res);
    }

    [HttpGet("gpt")]
    public async Task<IActionResult> GetOrdersGptAsync([FromQuery] PaginatedRequest request)
    {
        if (request.Offset < 0) return BadRequest();

        var res = await _orderService.GetOrdersGptAsync(request.Offset, request.Limit);

        return Ok(res);
    }

    [HttpGet("stat")]
    public async Task<IActionResult> GetOrderStats()
    {

        var res = await _orderService.GetOrderStats();

        return Ok(res);
    }

    [HttpGet("stat/addresses")]
    public async Task<IActionResult> GetAddressOrderStats([FromQuery] PaginatedRequest request)
    {
        if (request.Offset < 0) return BadRequest();

        var res = await _orderService.GetAllAddressOrderStats(request.Offset, request.Limit);

        return Ok(res);
    }

    [HttpGet("stat/address/{address}")]
    public async Task<IActionResult> GetAddressOrderStats(string address)
    {

        var res = await _orderService.GetAddressOrderStats(address);

        return Ok(res);
    }
}