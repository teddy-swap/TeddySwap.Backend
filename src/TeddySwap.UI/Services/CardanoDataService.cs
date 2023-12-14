using Microsoft.EntityFrameworkCore;
using TeddySwap.Data;

namespace TeddySwap.UI.Services;

public class HeartbeatEventArgs : EventArgs
{
    public ulong BlockNumber { get; set; }
}

public class CardanoDataService()
{
    public ulong CurrentBlockNumber { get; set; }
    public event EventHandler<HeartbeatEventArgs>? Heartbeat;

    public void TriggerHeartbeat(HeartbeatEventArgs e)
    {
        Heartbeat?.Invoke(this, e);
    }
}