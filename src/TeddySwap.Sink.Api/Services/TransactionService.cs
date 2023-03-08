using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeddySwap.Common.Models;
using TeddySwap.Common.Models.CardanoDbSync;
using TeddySwap.Common.Enums;
using TeddySwap.Common.Models.Request;
using TeddySwap.Common.Models.Response;
using TeddySwap.Sink.Api.Models;
using TeddySwap.Sink.Data;
using TeddySwap.Common.Services;

namespace TeddySwap.Sink.Api.Services;

public class TransactionService
{
    private readonly ILogger<TransactionService> _logger;
    private readonly CardanoDbSyncContext _dbContext;
    private readonly TeddySwapITNRewardSettings _settings;
    private readonly ByteArrayService _byteArrayService;

    public TransactionService(
        ILogger<TransactionService> logger,
        CardanoDbSyncContext dbContext,
        IOptions<TeddySwapITNRewardSettings> settings,
        ByteArrayService byteArrayService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _settings = settings.Value;
        _byteArrayService = byteArrayService;
    }

}