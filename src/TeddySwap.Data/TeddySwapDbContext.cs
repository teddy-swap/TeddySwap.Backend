using Microsoft.EntityFrameworkCore;
using TeddySwap.Data.Models;
using TeddySwap.Data.Models.Reducers;

namespace TeddySwap.Data;

public class TeddySwapDbContext(DbContextOptions<TeddySwapDbContext> options) : DbContext(options)
{
    public DbSet<Block> Blocks { get; set; }
    public DbSet<TransactionOutput> TransactionOutputs { get; set; }
    public DbSet<LovelaceByAddressItem> LovelaceByAddress { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LovelaceByAddressItem>().HasKey(item => new { item.Address, item.BlockNumber, item.Slot });
        modelBuilder.Entity<TransactionOutput>().HasKey(item => new { item.Id, item.Index });
        modelBuilder.Entity<TransactionOutput>().OwnsOne(item => item.Amount);
        modelBuilder.Entity<Block>().HasKey(item => new { item.Id, item.Slot });
        base.OnModelCreating(modelBuilder);
    }
}
