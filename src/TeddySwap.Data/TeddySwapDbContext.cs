using Microsoft.EntityFrameworkCore;

namespace TeddySwap.Data;

public class TeddySwapDbContext(DbContextOptions<TeddySwapDbContext> options) : DbContext(options)
{
    public DbSet<Block> Blocks { get; set; }
}
