namespace TeddySwap.UI.Services;

public class TeddySwapCalculatorService
{
    private double _adaValue { get; set; } = 0.3;
    private double _tokenXValue { get; set; } = 0.2;
    private decimal _randomConversionRate { get; set; } = 1.216220304M;

    public double ConvertToAda(double tokenAmount) => tokenAmount * _adaValue;

    public decimal CalculatePriceImpact(decimal tokenAmount) => tokenAmount / 50_000M;

    public decimal ConvertToTokenX(decimal tokenAmount) => tokenAmount * _randomConversionRate;

    public decimal ConvertToTokenY(decimal tokenAmount) => tokenAmount / _randomConversionRate;
}
