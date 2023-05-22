using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Orders;

public partial class Orders
{
    private string _searchValue { get; set; } = string.Empty;
    private ActionType ActionType { get; set; } = ActionType.Swap;
    private Status Status { get; set; } = Status.Complete;
    private IEnumerable<Order>? _orders { get; set; }

    protected override void OnInitialized()
    {
        _orders = new List<Order>
        {
            new()
            {
                TokenOne = new() { Logo = "../images/tokens/tedyt.png", Name = "TEDYt" },
                TokenTwo = new() { Logo = "../images/tokens/token-ada.svg", Name = "ADA" },
                TokenOneAmount = 356.24M,
                TokenTwoAmount = 86,
                ActionType = ActionType.Swap,
                DateTime = new DateTime(2023, 5, 22, 7, 47, 0),
                Status = Status.Complete
            },
            new()
            {
                TokenOne = new() { Logo = "../images/tokens/usdt.png", Name = "USDt" },
                TokenTwo = new() { Logo = "../images/tokens/token-ada.svg", Name = "ADA" },
                TokenOneAmount = 356.24M,
                TokenTwoAmount = 86,
                ActionType = ActionType.Swap,
                DateTime = new DateTime(2023, 5, 22, 7, 47, 0),
                Status = Status.Complete
            },
            new()
            {
                TokenOne = new() { Logo = "../images/tokens/djed.png", Name = "DJEDt" },
                TokenTwo = new() { Logo = "../images/tokens/tedyt.png", Name = "TEDYt" },
                TokenOneAmount = 356.24M,
                TokenTwoAmount = 86,
                ActionType = ActionType.AddLiquidity,
                DateTime = new DateTime(2023, 5, 22, 7, 47, 0),
                Status = Status.Pending
            },
            new()
            {
                TokenOne = new() { Logo = "../images/tokens/tedyt.png", Name = "TEDYt" },
                TokenTwo = new() { Logo = "../images/tokens/sundaet.png", Name = "SUNDAEt" },
                TokenOneAmount = 356.24M,
                TokenTwoAmount = 86,
                ActionType = ActionType.CreateLiquidity,
                DateTime = new DateTime(2023, 5, 22, 7, 47, 0),
                Status = Status.Complete
            },
            new()
            {
                TokenTwo = new() { Logo = "../images/tokens/token-ada.svg", Name = "ADA" },
                TokenOne = new() { Logo = "../images/tokens/usdt.png", Name = "USDt" },
                TokenOneAmount = 356.24M,
                TokenTwoAmount = 86,
                ActionType = ActionType.Swap,
                DateTime = new DateTime(2023, 5, 22, 7, 47, 0),
                Status = Status.Canceled
            }
        };
    }
}
