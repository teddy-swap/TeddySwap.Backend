using Microsoft.EntityFrameworkCore;
using TeddySwap.Common.Models;

namespace TeddySwap.Sink.Data;

public class TeddySwapBadgerAddressSinkDbContext : TeddySwapSinkCoreDbContext
{

    #region TeddySwap Models
    public DbSet<BadgerAddressVerification> BadgerAddressVerifications => Set<BadgerAddressVerification>();
    #endregion

    public TeddySwapBadgerAddressSinkDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BadgerAddressVerification>().HasKey(mt => new { mt.Address, mt.TxHash });
        base.OnModelCreating(modelBuilder);
    }
}