using System.Text.Json;
using TeddySwap.Common.Models;

namespace TeddySwap.UI.Services;

public class NftService
{
    private const string ROUND_ONE_POLICY_ID = "ab182ed76b669b49ee54a37dee0d0064ad4208a859cc4fdf3f906d87";
    private const string ROUND_TWO_POLICY_ID = "da3562fad43b7759f679970fb4e0ec07ab5bebe5c703043acda07a3c";
    private readonly IHttpClientFactory _clientFactory;
    public IEnumerable<TbcNft>? RoundOneNfts { get; set; }
    public IEnumerable<TbcNft>? RoundTwoNfts  {get; set; }

    public NftService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task InitializeNftDataAsync()
    {
        RoundOneNfts = JsonSerializer.Deserialize<IEnumerable<TbcNft>>(await File.ReadAllTextAsync(Path.Combine("./wwwroot", "teddybearsclub1.json")));
        RoundTwoNfts = JsonSerializer.Deserialize<IEnumerable<TbcNft>>(await File.ReadAllTextAsync(Path.Combine("./wwwroot", "teddybearsclub2.json")));
    }

    public TbcNft? GetRoundOneNft(string nftName) => RoundOneNfts?.Where(nft => nft.AssetName == nftName).FirstOrDefault();

    public TbcNft? GetRoundTwoNft(string nftName) => RoundTwoNfts?.Where(nft => nft.AssetName == nftName).FirstOrDefault();

    public TbcNft? GetNft(string policyId, string nftName)
    {
        return policyId switch 
        {
            ROUND_ONE_POLICY_ID => GetRoundOneNft(nftName),
            ROUND_TWO_POLICY_ID => GetRoundTwoNft(nftName),
            _ => throw new ArgumentException("Invalid Policy ID")
        };
    }
}