namespace TeddySwap.UI.Services;

public class TeddySwapCalculatorService
{
    private double _adaValue { get; set; } = 0.3;

    private double _tokenXValue { get; set; } = 0.2;

    public double ConvertToTokenX(double tokenAmount) => tokenAmount * _tokenXValue;

    public double ConvertToAda(double tokenAmount) => tokenAmount * _adaValue;

    public double CalculatePriceImpact(double tokenAmount) => tokenAmount / 50_000;
}
