using PallasDotnet.Models;

namespace TeddySwap.Sync.Reducers;

public interface IReducer
{
    Task RollForwardAsync(NextResponse response);
    Task RollBackwardAsync(NextResponse response);
}