using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TeddySwap.Data.Models;
using TeddySwap.Data.Models.Reducers;

namespace TeddySwap.Data;

public class TeddySwapDbContext(DbContextOptions<TeddySwapDbContext> options, IConfiguration configuration) : DbContext(options)
{
    private readonly IConfiguration _configuration = configuration;

    public DbSet<Block> Blocks { get; set; }
    public DbSet<TransactionOutput> TransactionOutputs { get; set; }
    public DbSet<LovelaceByAddressItem> LovelaceByAddress { get; set; }
    public DbSet<LiquidityByAddressItem> LiquidityByAddress { get; set; }
    public DbSet<YieldRewardByAddress> YieldRewardByAddress { get; set; }
    public DbSet<LedgerStateByAddress> LedgerStateByAddress { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_configuration.GetConnectionString("TeddySwapContextSchema"));
        modelBuilder.Entity<LovelaceByAddressItem>().HasKey(item => new { item.Address, item.BlockNumber, item.Slot });
        modelBuilder.Entity<LiquidityByAddressItem>().HasKey(item => new { item.Address, item.BlockNumber, item.Slot });
        modelBuilder.Entity<YieldRewardByAddress>().HasKey(item => new { item.Address, item.BlockNumber, item.Slot, item.PoolId });
        modelBuilder.Entity<LedgerStateByAddress>().HasKey(item => new { item.Address, item.BlockNumber, item.Slot });
        modelBuilder.Entity<TransactionOutput>().HasKey(item => new { item.Id, item.Index });
        modelBuilder.Entity<TransactionOutput>().OwnsOne(item => item.Amount);
        modelBuilder.Entity<Block>().HasKey(item => new { item.Id, item.Slot });
    }
}
