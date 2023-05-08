using Microsoft.EntityFrameworkCore;
using TeddySwap.Common.Models;

namespace TeddySwap.Sink.Data;

public class TeddySwapNftSinkDbContext : TeddySwapSinkCoreDbContext
{

    #region TeddySwap Models
    public DbSet<MintTransaction> MintTransactions => Set<MintTransaction>();
    public DbSet<NftOwner> NftOwners => Set<NftOwner>();
    #endregion

    public TeddySwapNftSinkDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MintTransaction>().HasKey(mt => new { mt.PolicyId, mt.TokenName, mt.TxHash });
        modelBuilder.Entity<NftOwner>().HasKey(nft => new { nft.PolicyId, nft.TokenName });
        base.OnModelCreating(modelBuilder);
    }
}