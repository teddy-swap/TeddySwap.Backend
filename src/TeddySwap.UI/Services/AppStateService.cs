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

    private int _liquidityValue;

    public int LiquidityValue
    {
        get => _liquidityValue;
        set
        {
            _liquidityValue = value;
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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
