using TeddySwap.Sink.Models.Models;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

public class DbContextAttribute : Attribute
{
    public ICollection<DbContextVariant> Variants { get; private set; }

    public DbContextAttribute(params DbContextVariant[] variants)
    {
        Variants = variants.ToList();
    }
}