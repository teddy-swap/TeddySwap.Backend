using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Enums;
using TeddySwap.Common.Models;
using TeddySwap.Common.Models.Response;
using TeddySwap.Sink.Api.Models;
using TeddySwap.Sink.Data;

namespace TeddySwap.Sink.Api.Services;

public class OrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly TeddySwapSinkDbContext _dbContext;
    private readonly TeddySwapITNRewardSettings _settings;
    private readonly IMemoryCache _memoryCache;

    public OrderService(
        ILogger<OrderService> logger,
        TeddySwapSinkDbContext dbContext,
        IMemoryCache memoryCache,
        IOptions<TeddySwapITNRewardSettings> settings)
    {
        _logger = logger;
        _dbContext = dbContext;
        _settings = settings.Value;
        _memoryCache = memoryCache;
    }


    public async Task<PaginatedOrderResponse> GetOrdersAsync(int offset, int limit)
    {
        List<OrderResponse> orders = await _dbContext.Orders
            .Include(o => o.Block)
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .OrderBy(o => o.Slot)
            .Skip(offset)
            .Take(limit)
            .Select(o => new OrderResponse()
            {
                Address = o.UserAddress,
                OrderType = o.OrderType,
                Slot = o.Slot,
                BlockNumber = o.Block.BlockNumber,
                OrderX = o.OrderX.ToString(),
                OrderY = o.OrderY.ToString(),
                OrderLq = o.OrderLq.ToString()
            })
            .ToListAsync();

        int totalOrders = await _dbContext.Orders
            .Include(o => o.Block)
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .CountAsync();

        return new()
        {
            Result = orders,
            TotalCount = totalOrders,
        };
    }


    public async Task<PaginatedOrderResponse> GetAddressOrdersAsync(string address)
    {
        List<OrderResponse> orders = await _dbContext.Orders
            .Include(o => o.Block)
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .Where(o => o.UserAddress == address)
            .OrderBy(o => o.Slot)
            .Select(o => new OrderResponse()
            {
                Address = o.UserAddress,
                OrderType = o.OrderType,
                Slot = o.Slot,
                BlockNumber = o.Block.BlockNumber,
                OrderX = o.OrderX.ToString(),
                OrderY = o.OrderY.ToString(),
                OrderLq = o.OrderLq.ToString()
            })
            .ToListAsync();

        int totalOrders = await _dbContext.Orders
            .Include(o => o.Block)
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .CountAsync();

        return new()
        {
            Result = orders,
            TotalCount = totalOrders,
        };
    }

    public async Task<PaginatedOrderGptResponse> GetOrdersGptAsync(int offset, int limit)
    {
        List<OrderGptResponse> orders = await _dbContext.Orders
            .Include(o => o.Block)
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .OrderBy(o => o.Slot)
            .Skip(offset)
            .Take(limit)
            .Select(o => new OrderGptResponse()
            {
                Prompt = JsonSerializer.Serialize(new OrderResponse
                {
                    Address = o.UserAddress,
                    OrderType = o.OrderType,
                    Slot = o.Slot,
                    BlockNumber = o.Block.BlockNumber,
                    OrderX = o.OrderX.ToString(),
                    OrderY = o.OrderY.ToString(),
                    OrderLq = o.OrderLq.ToString()
                }, new JsonSerializerOptions()),
                Completion = ""
            })
            .ToListAsync();
        int totalOrders = await _dbContext.Orders
            .Include(o => o.Block)
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .CountAsync();

        return new()
        {
            Result = orders,
            TotalCount = totalOrders,
        };
    }

    public async Task<AddressOrderStatResponse> GetAddressOrderStats(string addr)
    {
        OrderStatResponse overallStats = await GetOrderStats();
        var orders = await _dbContext.Orders
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .Where(o => o.UserAddress == addr)
            .OrderBy(o => o.Slot)
            .ToListAsync();

        var firstOrder = orders.FirstOrDefault();
        var lastOrder = orders.LastOrDefault();

        if (firstOrder == null || lastOrder == null)
        {
            return new AddressOrderStatResponse();
        }

        var orderStat = new AddressOrderStatResponse
        {
            Address = addr,
            AvgInterval = CalculateAverageOrderInterval(orders),
            AvgPerDay = CalculateAverageOrderPerDay(orders, firstOrder.Slot, lastOrder.Slot),
            AvgPerHour = CalculateAverageOrderPerHour(orders, firstOrder.Slot, lastOrder.Slot),
            BotScore = 0
        };

        orderStat.BotScore = CalculateBotLikelihood(orderStat, overallStats);

        return orderStat;
    }

    public async Task<List<AddressOrderStatResponse>> GetAllAddressOrderStats(int offset, int limit)
    {
        OrderStatResponse overallStats = await GetOrderStats();

        var groupedOrders = (await _dbContext.Orders
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .GroupBy(o => o.UserAddress)
            .ToListAsync())
            .Where(g => g.Count() >= 25 && ((g.Max(o => o.Slot) - g.Min(o => o.Slot)) / 86400m) >= 2);

        var addressStats = new List<AddressOrderStatResponse>();

        foreach (var group in groupedOrders)
        {
            var orders = group.OrderBy(o => o.Slot).ToList();

            var firstOrder = orders.FirstOrDefault();
            var lastOrder = orders.LastOrDefault();

            if (firstOrder == null || lastOrder == null)
            {
                continue;
            }

            var orderStat = new AddressOrderStatResponse
            {
                Address = group.Key,
                AvgInterval = CalculateAverageOrderInterval(orders),
                AvgPerDay = CalculateAverageOrderPerDay(orders, firstOrder.Slot, lastOrder.Slot),
                AvgPerHour = CalculateAverageOrderPerHour(orders, firstOrder.Slot, lastOrder.Slot),
                BotScore = 0
            };

            orderStat.BotScore = CalculateBotLikelihood(orderStat, overallStats);

            addressStats.Add(orderStat);
        }

        addressStats = addressStats
            .OrderByDescending(s => s.BotScore)
            .Skip(offset)
            .Take(limit)
            .ToList();

        return addressStats;
    }

    public async Task<OrderStatResponse> GetOrderStats()
    {
        var cachedValue = await _memoryCache.GetOrCreateAsync("OrderStats", async entry =>
        {
            var orders = await _dbContext.Orders
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .OrderBy(o => o.Slot)
            .ToListAsync();

            var groups = orders
                .GroupBy(o => o.UserAddress)
                .Where(g => g.Count() >= 25 && ((g.Max(o => o.Slot) - g.Min(o => o.Slot)) / 86400m) >= 2)
                .Select(g => new
                {
                    Address = g.Key,
                    Orders = g.OrderBy(o => o.Slot).ToList(),
                    Intervals = g.OrderBy(o => o.Slot)
                                  .AsEnumerable()
                                  .Zip(g.OrderBy(o => o.Slot).Skip(1), (o1, o2) => o2.Slot - o1.Slot)
                                  .ToList(),
                    FirstOrder = g.Min(o => o.Slot),
                    LastOrder = g.Max(o => o.Slot)
                })
                .ToList();

            var averages = groups.Select(g => new
            {
                g.Address,
                AverageInterval = g.Intervals.Count > 0 ?
                    g.Intervals.Select(interval => (decimal)interval).Average() :
                    0,
                AverageOrdersPerDay = g.Orders.Count / ((g.LastOrder - g.FirstOrder) / 86400m),
                AverageOrdersPerHour = g.Orders.Count / ((g.LastOrder - g.FirstOrder) / 3600m)
            }).ToList();

            var totalCount = averages.Count;

            decimal averageInterval = totalCount > 0 ?
                averages.Select(a => a.AverageInterval).Average() :
                0;

            decimal averageOrdersPerDay = totalCount > 0 ?
                averages.Select(a => a.AverageOrdersPerDay).Average() :
                0;

            decimal averageOrdersPerHour = totalCount > 0 ?
                averages.Select(a => a.AverageOrdersPerHour).Average() :
                0;

            double stdInterval = totalCount > 0 ?
                Math.Sqrt(averages.Select(a => Math.Pow((double)a.AverageInterval - (double)averageInterval, 2)).Sum() / totalCount) :
                0;

            double stdPerDay = totalCount > 0 ?
                Math.Sqrt(averages.Select(a => Math.Pow((double)a.AverageOrdersPerDay - (double)averageOrdersPerDay, 2)).Sum() / totalCount) :
                0;

            double stdPerHour = totalCount > 0 ?
                Math.Sqrt(averages.Select(a => Math.Pow((double)a.AverageOrdersPerHour - (double)averageOrdersPerHour, 2)).Sum() / totalCount) :
                0;

            return new OrderStatResponse
            {
                AvgInterval = averageInterval,
                StdInterval = (decimal)stdInterval,
                AvgPerDay = averageOrdersPerDay,
                StdPerDay = (decimal)stdPerDay,
                AvgPerHour = averageOrdersPerHour,
                StdPerHour = (decimal)stdPerHour
            };

        });

        if (cachedValue == null)
        {
            throw new Exception("Failed to retrieve cached value.");
        }

        return cachedValue;
    }

    public async Task<decimal> GetMedianTransactionCountAsync()
    {
        var usersQuery = await _dbContext.Orders
            .Where(o => !_dbContext.BlacklistedAddresses.Any(b => b.Address == o.UserAddress))
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .GroupBy(o => o.UserAddress)
            .Select(g => new
            {
                TestnetAddress = g.Key,
                Total = g.Count(o => o.OrderType != OrderType.Unknown),
            })
            .Where(u => u.Total > 10)
            .OrderBy(u => u.Total)
            .ToListAsync();

        if (usersQuery.Count == 0)
        {
            return 0;
        }

        int count = usersQuery.Count;
        int middleIndex = count / 2;

        if (count % 2 == 0)
        {
            // if there are even number of elements, take the average of the middle two elements
            decimal median = (usersQuery[middleIndex - 1].Total + usersQuery[middleIndex].Total) / 2m;
            return median;
        }
        else
        {
            // if there are odd number of elements, take the middle element
            decimal median = usersQuery[middleIndex].Total;
            return median;
        }
    }

    private static decimal CalculateAverageOrderInterval(List<Order> orders)
    {
        if (orders.Count <= 1)
        {
            return 0;
        }

        var intervals = new List<decimal>();

        for (int i = 1; i < orders.Count; i++)
        {
            intervals.Add(orders[i].Slot - orders[i - 1].Slot);
        }

        return intervals.Average();
    }

    private static decimal CalculateAverageOrderPerDay(List<Order> orders, ulong firstSlot, ulong lastSlot)
    {
        var days = (lastSlot - firstSlot) / 86400m;
        return orders.Count / days;
    }

    private static decimal CalculateAverageOrderPerHour(List<Order> orders, ulong firstSlot, ulong lastSlot)
    {
        var hours = (lastSlot - firstSlot) / 3600m;
        return orders.Count / hours;
    }

    private static decimal CalculateBotLikelihood(AddressOrderStatResponse userStats, OrderStatResponse overallStats)
    {
        // Calculate the deviations for each statistic for the user
        decimal intervalDeviation = overallStats.AvgInterval - userStats.AvgInterval;
        decimal perDayDeviation = userStats.AvgPerDay - overallStats.AvgPerDay;
        decimal perHourDeviation = userStats.AvgPerHour - overallStats.AvgPerHour;

        // Calculate the signed z-scores based on the deviations
        decimal signedZInterval = intervalDeviation > 0 ? intervalDeviation / overallStats.StdInterval : 0;
        decimal signedZPerDay = perDayDeviation > 0 ? perDayDeviation / overallStats.StdPerDay : 0;
        decimal signedZPerHour = perHourDeviation > 0 ? perHourDeviation / overallStats.StdPerHour : 0;

        // Calculate the Euclidean distance between the user's signed z-scores and the mean signed z-scores
        double distance = Math.Sqrt(Math.Pow((double)signedZInterval, 2) + Math.Pow((double)signedZPerDay, 2) + Math.Pow((double)signedZPerHour, 2));

        // Map the distance to a likelihood score between 0% and 100%
        decimal likelihood = Math.Round((1 - (decimal)Math.Exp(-0.1 * distance)) * 100, 2);

        return likelihood;
    }
}