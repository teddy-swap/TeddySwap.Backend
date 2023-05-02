using Microsoft.AspNetCore.Components;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Pages.Liquidity;

public partial class Liquidity
{
    private List<TokenPairDetails> _allTokenPairs { get; set; } = new();

    private List<TokenPairDetails> _userTokenPairs { get; set; } = new();

	protected override void OnInitialized()
	{
        _allTokenPairs = new()
        {
            new()
            {
                TokenPair = new()
                {
                    new Token() { Name = "TEDYt", Logo = "../images/tokens/tedyt.png" },
                    new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" }
                },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 1
            },
            new()
            {
                TokenPair = new()
                {
                    new Token() { Name = "USDt", Logo = "../images/tokens/usdt.png" },
                    new Token() { Name = "DJED", Logo = "../images/tokens/djed.png" }
                },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 2
            },
            new()
            {
                TokenPair = new()
                {
                    new Token() { Name = "SUNDAEt", Logo = "../images/tokens/sundaet.png" },
                    new Token() { Name = "TEDYt", Logo = "../images/tokens/tedyt.png" }
                },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 3
            },
            new()
            {
                TokenPair = new()
                {
                    new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" },
                    new Token() { Name = "USDt", Logo = "../images/tokens/usdt.png" }
                },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 4
            },
            new()
            {
                TokenPair = new()
                {
                    new Token() { Name = "DRIPt", Logo = "../images/tokens/dript.png" },
                    new Token() { Name = "WRTt", Logo = "../images/tokens/wrtt.png" }
                },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 5
            },
            new()
            {
                TokenPair = new()
                {
                    new Token() { Name = "USDt", Logo = "../images/tokens/usdt.png" },
                    new Token() { Name = "TEDYt", Logo = "../images/tokens/tedyt.png" }
                },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 6
            }
        };

         _userTokenPairs = new()
        {
            new()
            {
                TokenPair = new()
                {
                    new Token() { Name = "SUNDAEt", Logo = "../images/tokens/sundaet.png" },
                    new Token() { Name = "TEDYt", Logo = "../images/tokens/tedyt.png" }
                },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 3
            },
            new()
            {
                TokenPair = new()
                {
                    new Token() { Name = "TEDYt", Logo = "../images/tokens/tedyt.png" },
                    new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" }
                },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 1
            },
            new()
            {
                TokenPair = new()
                {
                    new Token() { Name = "ADA", Logo = "../images/tokens/token-ada.svg" },
                    new Token() { Name = "USDt", Logo = "../images/tokens/usdt.png" }
                },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 4
            },
            new()
            {
                TokenPair = new()
                {
                    new Token() { Name = "USDt", Logo = "../images/tokens/usdt.png" },
                    new Token() { Name = "TEDYt", Logo = "../images/tokens/tedyt.png" }
                },
                TVL = 26.29,
                Volume24H = 450.88,
                Volume7D = 8_230_456,
                Fee = 0.3,
                APR = 6.5,
                Number = 6
            }
        };
	}
}
