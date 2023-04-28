using Microsoft.EntityFrameworkCore;
using TeddySwap.Common.Models;

namespace TeddySwap.Sink.Data;

public class TeddySwapOrderSinkDbContext : TeddySwapSinkCoreDbContext
{

    #region TeddySwap Models
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Price> Prices => Set<Price>();
    public DbSet<AddressVerification> AddressVerifications => Set<AddressVerification>();
    public DbSet<BlacklistedAddress> BlacklistedAddresses => Set<BlacklistedAddress>();
    #endregion

    public TeddySwapOrderSinkDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().HasKey(order => new { order.TxHash, order.Index });
        modelBuilder.Entity<Price>().HasKey(price => new { price.TxHash, price.Index });
        modelBuilder.Entity<BlacklistedAddress>().HasKey(ba => ba.Address);
        modelBuilder.Entity<AddressVerification>().HasKey(a => a.TestnetAddress);

        // Relations

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Slot)
            .IsUnique(false);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.UserAddress)
            .IsUnique(false);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.BatcherAddress)
            .IsUnique(false);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.OrderType)
            .IsUnique(false);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Slot)
            .IsUnique(false);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Slot)
            .IsUnique(false);

        base.OnModelCreating(modelBuilder);
    }
}