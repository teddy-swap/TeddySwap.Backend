using Microsoft.EntityFrameworkCore;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Data;

namespace TeddySwap.Sink.Api.Services;

public class AddressVerificationService
{
    private readonly TeddySwapOrderSinkDbContext _dbContext;

    public AddressVerificationService(TeddySwapOrderSinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> GetMainnetAddressByTestnetAddressAsync(string testnetAddress)
    {
        return (await _dbContext.AddressVerifications.Where(av => av.TestnetAddress == testnetAddress).FirstOrDefaultAsync())?.MainnetAddress;
    }

    public async Task AddVerificationAsync(string testnetAddress, string mainnetAddress, string testnetSignedData)
    {
        AddressVerification? av = await _dbContext.AddressVerifications.Where(av => av.TestnetAddress == testnetAddress).FirstOrDefaultAsync();

        if (av is not null)
            _dbContext.AddressVerifications.Remove(av);

        await _dbContext.AddressVerifications.AddAsync(new()
        {
            TestnetAddress = testnetAddress,
            MainnetAddress = mainnetAddress,
            TestnetSignedData = testnetSignedData,
            MainnetSignedData = string.Empty
        });

        await _dbContext.SaveChangesAsync();
    }
}