using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Enums;
using TeddySwap.Common.Models;
using TeddySwap.Common.Models.Response;
using TeddySwap.Sink.Api.Models;
using TeddySwap.Sink.Data;

namespace TeddySwap.Sink.Api.Services;

public class LeaderboardService
{
    private readonly ILogger<LeaderboardService> _logger;
    private readonly TeddySwapSinkDbContext _dbContext;
    private readonly TeddySwapITNRewardSettings _settings;
    private readonly AssetService _assetService;
    private readonly IMemoryCache _memoryCache;

    public LeaderboardService(
        ILogger<LeaderboardService> logger,
        TeddySwapSinkDbContext dbContext,
        AssetService assetService,
        IOptions<TeddySwapITNRewardSettings> settings,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _dbContext = dbContext;
        _assetService = assetService;
        _settings = settings.Value;
        _memoryCache = memoryCache;
    }

    public int GetRewardAmount(LeaderBoardType type)
    {
        int totalReward = type switch
        {
            LeaderBoardType.Users => _settings.UserReward,
            LeaderBoardType.Badgers => _settings.BatcherReward,
            _ => _settings.TotalReward
        };

        return totalReward;
    }

    public async Task<PaginatedLeaderBoardResponse?> GetUserLeaderboardAsync(int offset, int limit)
    {
        var usersQuery = _dbContext.Orders
            .Where(o => !_dbContext.BlacklistedAddresses.Any(b => b.Address == o.UserAddress))
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .GroupBy(o => o.UserAddress)
            .Select(g => new
            {
                TestnetAddress = g.Key,
                Total = g.Count(o => o.OrderType != OrderType.Unknown),
                Deposit = g.Count(o => o.OrderType == OrderType.Deposit),
                Redeem = g.Count(o => o.OrderType == OrderType.Redeem),
                Swap = g.Count(o => o.OrderType == OrderType.Swap),
            })
            .Where(u => u.Total > 0);

        var paginatedUsersQuery = usersQuery
            .OrderByDescending(u => u.Total)
            .ThenBy(u => u.TestnetAddress)
            .Skip(offset)
            .Take(limit);

        var usersWithMainnetAddress = paginatedUsersQuery
            .GroupJoin(_dbContext.AddressVerifications,
                entry => entry.TestnetAddress,
                verification => verification.TestnetAddress,
                (entry, verifications) => new { Entry = entry, Verifications = verifications })
            .SelectMany(x => x.Verifications.DefaultIfEmpty(),
                (x, verification) => new
                {
                    x.Entry.TestnetAddress,
                    MainnetAddress = verification == null ? "" : verification.MainnetAddress,
                    x.Entry.Total,
                    x.Entry.Deposit,
                    x.Entry.Redeem,
                    x.Entry.Swap,
                });

        (int totalUsers, decimal totalPoints) = await GetUserTotalsAsync();
        int reward = GetRewardAmount(LeaderBoardType.Users);

        List<LeaderBoardResponse> users = (await usersWithMainnetAddress.ToListAsync())
            .Select((u, index) => new LeaderBoardResponse
            {
                TestnetAddress = u.TestnetAddress,
                MainnetAddress = u.MainnetAddress,
                Total = u.Total,
                Deposit = u.Deposit,
                Redeem = u.Redeem,
                Swap = u.Swap,
                Batch = 0,
                Rank = index + offset + 1,
                BaseRewardPercentage = u.Total / totalPoints,
                BaseReward = u.Total / totalPoints * reward,
            })
            .ToList();

        return new()
        {
            Result = users,
            TotalCount = totalUsers,
            TotalAmount = (int)totalPoints
        };
    }

    public async Task<PaginatedLeaderBoardResponse?> GetBadgerLeaderboardAsync(int offset, int limit)
    {
        var badgersQuery = _dbContext.Orders
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .Where(o => o.BatcherAddress != null && !_dbContext.BlacklistedAddresses.Any(b => b.Address == o.BatcherAddress))
            .GroupBy(o => o.BatcherAddress)
            .Select(g => new
            {
                TestnetAddress = g.Key ?? "",
                Total = g.Count(),
            });

        var paginatedBadgersQuery = badgersQuery
            .OrderByDescending(u => u.Total)
            .ThenBy(u => u.TestnetAddress)
            .Skip(offset)
            .Take(limit);

        var badgersWithMainnetAddress = paginatedBadgersQuery
            .GroupJoin(_dbContext.AddressVerifications,
                entry => entry.TestnetAddress,
                verification => verification.TestnetAddress,
                (entry, verifications) => new { Entry = entry, Verifications = verifications })
            .SelectMany(x => x.Verifications.DefaultIfEmpty(),
                (x, verification) => new
                {
                    x.Entry.TestnetAddress,
                    MainnetAddress = verification == null ? "" : verification.MainnetAddress,
                    x.Entry.Total,
                });

        (int totalBadgers, decimal totalPoints) = await GetBadgerTotalsAsync();

        int reward = GetRewardAmount(LeaderBoardType.Badgers);

        List<LeaderBoardResponse> users = (await badgersWithMainnetAddress.ToListAsync())
            .Select((u, index) => new LeaderBoardResponse
            {
                TestnetAddress = u.TestnetAddress,
                MainnetAddress = u.MainnetAddress,
                Total = u.Total,
                Deposit = 0,
                Redeem = 0,
                Swap = 0,
                Batch = u.Total,
                Rank = index + offset + 1,
                BaseRewardPercentage = u.Total / totalPoints,
                BaseReward = u.Total / totalPoints * reward,
            })
            .ToList();

        return new()
        {
            Result = users,
            TotalCount = totalBadgers,
            TotalAmount = (int)totalPoints
        };
    }

    public async Task<PaginatedLeaderBoardResponse?> GetSingleUserAddressLeaderboardAsync(string bech32Address)
    {
        var usersQuery = _dbContext.Orders
           .Where(o => !_dbContext.BlacklistedAddresses.Any(b => b.Address == o.UserAddress))
           .Where(o => o.Slot <= _settings.ItnEndSlot)
           .GroupBy(o => o.UserAddress)
           .Select(g => new
           {
               TestnetAddress = g.Key,
               Total = g.Count(o => o.OrderType != OrderType.Unknown),
               Deposit = g.Count(o => o.OrderType == OrderType.Deposit),
               Redeem = g.Count(o => o.OrderType == OrderType.Redeem),
               Swap = g.Count(o => o.OrderType == OrderType.Swap),
           })
           .Where(u => u.Total > 0)
           .OrderByDescending(u => u.Total);

        var filteredUser = usersQuery
            .Where(u => u.TestnetAddress == bech32Address);

        var rankedUsers = filteredUser
            .Select(u => new
            {
                u.TestnetAddress,
                u.Deposit,
                u.Swap,
                u.Redeem,
                u.Total,
                Rank = usersQuery
                    .Where(x => x.Total > 0 && x.Total > usersQuery
                        .Where(y => y.TestnetAddress == u.TestnetAddress)
                        .Select(y => y.Total)
                        .FirstOrDefault())
                    .Count() + 1,
            });

        var userWithMainnetAddress = rankedUsers
            .GroupJoin(_dbContext.AddressVerifications,
                entry => entry.TestnetAddress,
                verification => verification.TestnetAddress,
                (entry, verifications) => new { Entry = entry, Verifications = verifications })
            .SelectMany(x => x.Verifications.DefaultIfEmpty(),
                (x, verification) => new
                {
                    x.Entry.TestnetAddress,
                    MainnetAddress = verification == null ? "" : verification.MainnetAddress,
                    x.Entry.Total,
                    x.Entry.Deposit,
                    x.Entry.Redeem,
                    x.Entry.Swap,
                    x.Entry.Rank
                });

        (int totalUsers, decimal totalPoints) = await GetUserTotalsAsync();
        int reward = GetRewardAmount(LeaderBoardType.Users);

        var user = await userWithMainnetAddress.FirstOrDefaultAsync();

        if (user is null) return null;

        LeaderBoardResponse leaderboardUser = new()
        {
            TestnetAddress = user.TestnetAddress,
            MainnetAddress = user.MainnetAddress,
            Total = user.Total,
            Deposit = user.Deposit,
            Redeem = user.Redeem,
            Swap = user.Swap,
            Batch = 0,
            Rank = user.Rank,
            BaseRewardPercentage = user.Total / totalPoints,
            BaseReward = user.Total / totalPoints * reward,
        };

        return new()
        {
            Result = new List<LeaderBoardResponse>() { leaderboardUser },
            TotalAmount = (int)totalPoints,
            TotalCount = totalUsers
        };
    }

    public async Task<PaginatedLeaderBoardResponse?> GetSingleBadgerAddressLeaderboardAsync(string bech32Address)
    {
        var badgersQuery = _dbContext.Orders
            .Where(b => b.BatcherAddress != null)
            .Where(o => !_dbContext.BlacklistedAddresses.Any(b => b.Address == o.BatcherAddress))
            .Where(o => o.Slot <= _settings.ItnEndSlot)
            .GroupBy(o => o.BatcherAddress)
            .Select(g => new
            {
                TestnetAddress = g.Key ?? "",
                Total = g.Count(),
            })
            .OrderByDescending(u => u.Total);


        var filteredBadger = badgersQuery
            .Where(u => u.TestnetAddress == bech32Address);

        var rankedBadgers = filteredBadger
             .Select(u => new
             {
                 u.TestnetAddress,
                 u.Total,
                 Rank = badgersQuery
                     .Where(x => x.Total > 0 && x.Total > badgersQuery
                         .Where(y => y.TestnetAddress == u.TestnetAddress)
                         .Select(y => y.Total)
                         .FirstOrDefault())
                     .Count() + 1,
             });

        var badgersWithMainnetAddress = rankedBadgers
            .GroupJoin(_dbContext.AddressVerifications,
                entry => entry.TestnetAddress,
                verification => verification.TestnetAddress,
                (entry, verifications) => new { Entry = entry, Verifications = verifications })
            .SelectMany(x => x.Verifications.DefaultIfEmpty(),
                (x, verification) => new
                {
                    x.Entry.TestnetAddress,
                    MainnetAddress = verification == null ? "" : verification.MainnetAddress,
                    x.Entry.Total,
                    x.Entry.Rank
                });

        (int totalBadgers, decimal totalPoints) = await GetBadgerTotalsAsync();
        int reward = GetRewardAmount(LeaderBoardType.Badgers);

        var badger = await badgersWithMainnetAddress.FirstOrDefaultAsync();

        if (badger is null) return null;

        LeaderBoardResponse leaderboardUser = new()
        {
            TestnetAddress = badger.TestnetAddress,
            MainnetAddress = badger.MainnetAddress,
            Total = badger.Total,
            Deposit = 0,
            Redeem = 0,
            Swap = 0,
            Batch = badger.Total,
            Rank = badger.Rank,
            BaseRewardPercentage = badger.Total / totalPoints,
            BaseReward = badger.Total / totalPoints * reward,
        };

        return new()
        {
            Result = new List<LeaderBoardResponse>() { leaderboardUser },
            TotalAmount = (int)totalPoints,
            TotalCount = totalBadgers
        };
    }

    public async Task<PaginatedLeaderBoardResponse?> GetMultipleAddressUserLeaderboardAsync(List<string> bech32Addresses)
    {
        var usersQuery = _dbContext.Orders
           .Where(o => o.Slot <= _settings.ItnEndSlot)
           .Where(o => !_dbContext.BlacklistedAddresses.Any(b => b.Address == o.UserAddress))
           .GroupBy(o => o.UserAddress)
           .Select(g => new
           {
               TestnetAddress = g.Key,
               Total = g.Count(o => o.OrderType != OrderType.Unknown),
               Deposit = g.Count(o => o.OrderType == OrderType.Deposit),
               Redeem = g.Count(o => o.OrderType == OrderType.Redeem),
               Swap = g.Count(o => o.OrderType == OrderType.Swap),
           })
           .Where(u => u.Total > 0)
           .OrderByDescending(u => u.Total);

        var filteredUser = usersQuery
            .Where(u => bech32Addresses.Contains(u.TestnetAddress));

        var usersWithMainnetAddress = filteredUser
            .GroupJoin(_dbContext.AddressVerifications,
                entry => entry.TestnetAddress,
                verification => verification.TestnetAddress,
                (entry, verifications) => new { Entry = entry, Verifications = verifications })
            .SelectMany(x => x.Verifications.DefaultIfEmpty(),
                (x, verification) => new
                {
                    x.Entry.TestnetAddress,
                    MainnetAddress = verification == null ? "" : verification.MainnetAddress,
                    x.Entry.Total,
                    x.Entry.Deposit,
                    x.Entry.Redeem,
                    x.Entry.Swap,
                });

        (int totalUsers, decimal totalPoints) = await GetUserTotalsAsync();
        int reward = GetRewardAmount(LeaderBoardType.Users);

        List<LeaderBoardResponse> users = (await usersWithMainnetAddress.ToListAsync())
            .Select(u => new LeaderBoardResponse
            {
                TestnetAddress = u.TestnetAddress,
                MainnetAddress = u.MainnetAddress,
                Total = u.Total,
                Deposit = u.Deposit,
                Redeem = u.Redeem,
                Swap = u.Swap,
                Batch = 0,
                Rank = 0,
                BaseRewardPercentage = u.Total / totalPoints,
                BaseReward = u.Total / totalPoints * reward,
            })
            .ToList();

        if (users is null) return null;

        LeaderBoardResponse combinedRewards = new()
        {
            TestnetAddress = "",
            MainnetAddress = "",
            Total = users.Sum(u => u.Total),
            Deposit = users.Sum(u => u.Deposit),
            Redeem = users.Sum(u => u.Redeem),
            Swap = users.Sum(u => u.Swap),
            Batch = 0,
            Rank = users.Average(u => u.Rank),
            BaseRewardPercentage = users.Sum(u => u.BaseRewardPercentage),
            BaseReward = users.Sum(u => u.BaseReward),
        };

        return new()
        {
            Result = new List<LeaderBoardResponse>() { combinedRewards },
            TotalAmount = (int)totalPoints,
            TotalCount = totalUsers
        };
    }

    public async Task<(int totalBadgers, decimal totalPoints)> GetBadgerTotalsAsync()
    {
        return await _memoryCache.GetOrCreateAsync<(int, decimal)>("BadgerTotals", async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(120));

            var totalBadgers = await _dbContext.Orders
                .Where(o => o.Slot <= _settings.ItnEndSlot)
                .Select(o => o.BatcherAddress)
                .Where(ba => ba != null)
                .Distinct()
                .Where(ba => !_dbContext.BlacklistedAddresses.Select(ta => ta.Address).Contains(ba))
                .CountAsync();

            var totalPoints = await _dbContext.Orders
                .Where(o => o.Slot <= _settings.ItnEndSlot)
                .Where(o => o.BatcherAddress != null && !_dbContext.BlacklistedAddresses.Any(b => b.Address == o.BatcherAddress))
                .CountAsync();

            return (totalBadgers, totalPoints);
        });
    }

    public async Task<(int totalUsers, decimal totalPoints)> GetUserTotalsAsync()
    {
        return await _memoryCache.GetOrCreateAsync<(int, decimal)>("UserTotals", async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(120));

            var totalUsers = await _dbContext.Orders
                .Where(o => o.Slot <= _settings.ItnEndSlot)
                .Select(o => o.UserAddress)
                .Distinct()
                .Where(ua => !_dbContext.BlacklistedAddresses.Select(ba => ba.Address).Contains(ua))
                .CountAsync();

            var totalPoints = await _dbContext.Orders
                .Where(o => o.Slot <= _settings.ItnEndSlot)
                .Where(o => !_dbContext.BlacklistedAddresses.Any(b => b.Address == o.UserAddress))
                .CountAsync();

            return (totalUsers, totalPoints);
        });
    }
}