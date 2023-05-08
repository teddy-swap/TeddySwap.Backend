using System.ComponentModel;
using System.Runtime.CompilerServices;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Services;

public class AppStateService : INotifyPropertyChanged
{
    private double _slippageToleranceValue;

    public double SlippageToleranceValue
    {
        get => _slippageToleranceValue;
        set
        {
            _slippageToleranceValue = value;
            OnPropertyChanged();
        }
    }

    private double _honeyValue;

    public double HoneyValue
    {
        get => _honeyValue;
        set
        {
            _honeyValue = value;
            OnPropertyChanged();
        }
    }

    private double _fromValue;

    public double FromValue
    {
        get => _fromValue;
        set
        {
            _fromValue = value;
            OnPropertyChanged();
        }
    }

    private double _toValue;

    public double ToValue
    {
        get => _toValue;
        set
        {
            _toValue = value;
            OnPropertyChanged();
        }
    }

    private double _liquidityTokenOneAmount;

    public double LiquidityTokenOneAmount
    {
        get => _liquidityTokenOneAmount;
        set
        {
            _liquidityTokenOneAmount = value;
            OnPropertyChanged();
        }
    }

    private double _liquidityTokenTwoAmount;

    public double LiquidityTokenTwoAmount
    {
        get => _liquidityTokenTwoAmount;
        set
        {
            _liquidityTokenTwoAmount = value;
            OnPropertyChanged();
        }
    }

    private double _liquidityFeePercentage;
    public double LiquidityFeePercentage
    {
        get => _liquidityFeePercentage;
        set
        {
            _liquidityFeePercentage = value;
            OnPropertyChanged();
        }
    }
    

    private Token? _fromCurrentlySelectedToken;

    public Token? FromCurrentlySelectedToken
    {
        get => _fromCurrentlySelectedToken;
        set
        {
            _fromCurrentlySelectedToken = value;
            OnPropertyChanged();
        }
    }

    private Token? _toCurrentlySelectedToken;

    public Token? ToCurrentlySelectedToken
    {
        get => _toCurrentlySelectedToken;
        set
        {
            _toCurrentlySelectedToken = value;
            OnPropertyChanged();
        }
    }

    private Token? _liquidityCurrentlySelectedTokenOne;

    public Token? LiquidityCurrentlySelectedTokenOne
    {
        get => _liquidityCurrentlySelectedTokenOne;
        set
        {
            _liquidityCurrentlySelectedTokenOne = value;
            OnPropertyChanged();
        }
    }

    private Token? _liquidityCurrentlySelectedTokenTwo;

    public Token? LiquidityCurrentlySelectedTokenTwo
    {
        get => _liquidityCurrentlySelectedTokenTwo;
        set
        {
            _liquidityCurrentlySelectedTokenTwo = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
