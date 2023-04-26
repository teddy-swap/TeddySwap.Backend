using Blazored.LocalStorage;
using Microsoft.JSInterop;
using TeddySwap.Common.Models;
using TeddySwap.UI.Models;

namespace TeddySwap.UI.Services;

// Scoped Service
public class CardanoWalletService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILocalStorageService _localStorage;

    public event EventHandler? ConnectionStateChange;
    public Guid SessionId { private set; get; } = Guid.NewGuid();
    public string? ConnectedAddress { get; set; }
    public CardanoWallet? ConnectedWallet { get; set; }

    public CardanoWalletService(IJSRuntime jSRuntime, ILocalStorageService localStorage)
    {
        _jsRuntime = jSRuntime;
        _localStorage = localStorage;
    }

    public async Task LoadStateFromStorageAsync()
    {
        if (await _localStorage.ContainKeyAsync("ConnectedWallet"))
        {
            CardanoWallet wallet = await _localStorage.GetItemAsync<CardanoWallet>("ConnectedWallet");
            await EnableAsync(wallet);
        }
    }

    public async Task<IEnumerable<CardanoWallet>> GetAvailableWalletsAsync()
    {
        ArgumentNullException.ThrowIfNull(_jsRuntime);
        return await _jsRuntime.InvokeAsync<IEnumerable<CardanoWallet>>("CardanoWalletService.getWallets");
    }

    public async Task<bool> EnableAsync(CardanoWallet wallet)
    {
        ArgumentNullException.ThrowIfNull(_jsRuntime);
        bool result = await _jsRuntime.InvokeAsync<bool>("CardanoWalletService.enableAsync", wallet.Id);

        if (result)
        {
            ConnectedAddress = await GetAddressAsync();
            ConnectedWallet = wallet;
            await _localStorage.SetItemAsync<CardanoWallet>("ConnectedWallet", ConnectedWallet);
            ConnectionStateChange?.Invoke(this, EventArgs.Empty);
        }
        
        SessionId = Guid.NewGuid();

        return result;
    }

    public async Task<string[]> GetUsedAddressesAsync()
    {
        ArgumentNullException.ThrowIfNull(_jsRuntime);
        return await _jsRuntime.InvokeAsync<string[]>("CardanoWalletService.getUsedAddressesAsync");
    }

    public async Task<CardanoSignedMessage> SignMessage(string message)
    {
        ArgumentNullException.ThrowIfNull(_jsRuntime);
        return await _jsRuntime.InvokeAsync<CardanoSignedMessage>("CardanoWalletService.signMessageAsync", message);
    }

    public async Task<string> GetAddressAsync()
    {
        ArgumentNullException.ThrowIfNull(_jsRuntime);
        return await _jsRuntime.InvokeAsync<string>("CardanoWalletService.getAddressAsync");
    }

    public async Task<string> GetStakeAddressAsync()
    {
        ArgumentNullException.ThrowIfNull(_jsRuntime);
        return await _jsRuntime.InvokeAsync<string>("CardanoWalletService.getStakeAddressAsync");
    }

    public async Task DisconnectAsync()
    {
        ArgumentNullException.ThrowIfNull(_jsRuntime);
        ConnectedAddress = string.Empty;
        await _jsRuntime.InvokeVoidAsync("CardanoWalletService.disconnect");
        await _localStorage.RemoveItemAsync("ConnectedWallet");
        ConnectionStateChange?.Invoke(this, EventArgs.Empty);
    }
}