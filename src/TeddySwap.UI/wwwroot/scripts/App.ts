import { Kupmios, Lucid, SignedMessage, fromText } from 'lucid-cardano';
import { Wallet } from './Types/Wallet';

declare global {
    interface Window {
        CardanoWalletService: {
            enableAsync: (walletId: string) => Promise<boolean>;
            disconnect: () => void;
            getWallets: () => Wallet[];
            getAddressAsync: () => Promise<string | undefined>;
            getUsedAddressesAsync: () => Promise<string[]>;
            signMessageAsync: (address: string, message: string) => Promise<SignedMessage | undefined>;
            getStakeAddressAsync: () => Promise<string | null | undefined>;
            lucid?: Lucid;
            walletApi?: any;
        }
    }
}

const getWallets = () => {
    const result: Wallet[] = [];
    const cardano = (window as any).cardano;
    for (const i in cardano) {
        const p = cardano[i];
        if (p.apiVersion != null && p.icon != null && p.name != null && i !== 'ccvault') {
            result.push({
                apiVersion: p.apiVersion,
                icon: p.icon,
                name: p.name,
                id: i.toString()
            });
        }
    }
    return result;
};

const enableAsync = async (walletId: string) => {
    try {
        const cardano = (window as any).cardano;
        const walletApi = await cardano[walletId].enable();
        window.CardanoWalletService.lucid = await Lucid.new(
            new Kupmios('https://kupo-preview-api-teddy-swap-preview-414e80.us1.demeter.run', 'wss://ogmios-preview-api-teddy-swap-preview-414e80.us1.demeter.run'),
            "Preview"
        );
        window.CardanoWalletService.lucid.selectWallet(walletApi);
        window.CardanoWalletService.walletApi = walletApi;
    }
    catch (ex) {
        console.error(ex);
    }
    finally {
        return window.CardanoWalletService.lucid != undefined;
    }
};

const getAddressAsync = async () => {
    return await window.CardanoWalletService.lucid?.wallet.address()
};

const getUsedAddressesAsync = async () => {
    if (window.CardanoWalletService.walletApi != null) {
        const cborAddresses: string[] = await window.CardanoWalletService.walletApi.getUsedAddresses();
        const addresses: string[] = cborAddresses.map(cA => window.CardanoWalletService.lucid?.utils.getAddressDetails(cA).address.bech32).filter(cA => cA) as string[];
        return addresses;
    }
    else
        return [];
}

const signMessageAsync = async (messageHex: string) => {
    const rewardAddress = await getStakeAddressAsync();
    return await window.CardanoWalletService.lucid?.wallet.signMessage(rewardAddress!, messageHex);
}

const getStakeAddressAsync = async () => {
    return await window.CardanoWalletService.lucid?.wallet.rewardAddress()
}

const disconnect = () => {
    delete window.CardanoWalletService.lucid;
    delete window.CardanoWalletService.walletApi;
}

window.CardanoWalletService = {
    enableAsync,
    disconnect,
    getWallets,
    getAddressAsync,
    getUsedAddressesAsync,
    signMessageAsync,
    getStakeAddressAsync
};