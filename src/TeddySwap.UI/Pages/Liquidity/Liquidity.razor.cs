using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class Liquidity
{
    private List<LiquidityData> _allLiquidityData { get; set; } = new();

    private List<UserLiquidityData> _userLiquidityData { get; set; } = new();

	protected override void OnInitialized()
	{
        _allLiquidityData = new()
        {
            new()
            {
                TokenPair = new() { Tokens = (
                    new Token() { Name = "TEDYt", Logo = "../images/tokens/tedyt.png" },
                    new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" }
                ) },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 1
            },
            new()
            {
                TokenPair = new() { Tokens = (
                    new Token() { Name = "iUSDt", Logo = "../images/tokens/usdt.png" },
                    new Token() { Name = "DJEDt", Logo = "../images/tokens/djed.png" }
                ) },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 2
            },
            new()
            {
                TokenPair = new() { Tokens = (
                    new Token() { Name = "SUNDAEt", Logo = "../images/tokens/sundaet.png" },
                    new Token() { Name = "TEDYt", Logo = "../images/tokens/tedyt.png" }
                ) },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 3
            },
            new()
            {
                TokenPair = new() { Tokens = (
                    new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" },
                    new Token() { Name = "iUSDt", Logo = "../images/tokens/usdt.png" }
                ) },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 4
            },
            new()
            {
                TokenPair = new() { Tokens = (
                    new Token() { Name = "DRIPt", Logo = "../images/tokens/dript.png" },
                    new Token() { Name = "WRTt", Logo = "../images/tokens/wrtt.png" }
                ) },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 5
            },
            new()
            {
                TokenPair = new() { Tokens = (
                    new Token() { Name = "iUSDt", Logo = "../images/tokens/usdt.png" },
                    new Token() { Name = "TEDYt", Logo = "../images/tokens/tedyt.png" }
                ) },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 6
            }
        };

        _userLiquidityData = new()
        {
            new()
            {
                TokenOneInfo = new()
                {
                    Token = new Token() { Name = "iUSDt", Logo = "../images/tokens/usdt.png" },
                    AmountAdded = 500,
                    CurrentBalance = 1000
                },
                TokenTwoInfo = new()
                {
                    Token = new Token() { Name = "TEDYt", Logo = "../images/tokens/tedyt.png" },
                    AmountAdded = 100,
                    CurrentBalance = 98
                },
                FeeShare = 0.05,
                ImpermanentLoss = 20,
                Number = 1
            },
            new()
            {
                TokenOneInfo = new()
                {
                    Token = new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" },
                    AmountAdded = 10_300,
                    CurrentBalance = 2_500
                },
                TokenTwoInfo = new()
                {
                    Token = new Token() { Name = "DJEDt", Logo = "../images/tokens/djed.png" },
                    AmountAdded = 3_000,
                    CurrentBalance = 10_000
                },
                FeeShare = 0.04,
                ImpermanentLoss = 10.67M,
                Number = 2
            }
        };

        
	}
}
