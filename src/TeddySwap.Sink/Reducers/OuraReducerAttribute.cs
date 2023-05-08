using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

public class OuraReducerAttribute : Attribute
{
    public ICollection<OuraVariant> Variants { get; private set; }

    public OuraReducerAttribute(params OuraVariant[] variants)
    {
        Variants = variants.ToList();
    }
}