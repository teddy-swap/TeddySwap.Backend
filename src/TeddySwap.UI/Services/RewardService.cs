using TeddySwap.Common.Models;

namespace TeddySwap.UI.Services;

public class RewardService
{
    private const double ROUND_ONE_MULTIPLIER = 2.75;
    private const int EFFECTIVE_NFTS = 4_246;
    private const int AVAILABLE_REWARDS = 33_453_000;
    private const string ROUND_ONE_POLICY_ID = "ab182ed76b669b49ee54a37dee0d0064ad4208a859cc4fdf3f906d87";
    private const string ROUND_TWO_POLICY_ID = "da3562fad43b7759f679970fb4e0ec07ab5bebe5c703043acda07a3c";
    private readonly NftService _nftService;

    public RewardService(NftService nftService) 
    {
        _nftService = nftService;
    }

    public NftRewardBreakdown CalculateNfTReward(string policyId, string nftName, int mintOrder = 0) 
    {
        return policyId switch
        {
            ROUND_ONE_POLICY_ID => CalculateRoundOneNftReward(nftName),
            ROUND_TWO_POLICY_ID => CalculateRoundTwoNftReward(nftName, mintOrder),
            _ => throw new ArgumentException("Invalid Policy ID")
        };
    }

    public NftRewardBreakdown CalculateRoundOneNftReward(string nftName) 
    {
        TbcNft? nft = _nftService.GetNft(ROUND_ONE_POLICY_ID, nftName);
        int rank = int.Parse(nft?.RarityRank ?? "0");
        int baseReward = GetRoundOneBaseReward(rank);
        decimal bonusReward = baseReward * (decimal)0.10;
        decimal earlySupporterBonus = AVAILABLE_REWARDS * ((decimal)ROUND_ONE_MULTIPLIER / EFFECTIVE_NFTS);
        
        return new() 
        {
            BaseReward = baseReward,
            BonusReward = bonusReward,
            EarlySupporterBonus = earlySupporterBonus,
            TotalReward = baseReward + bonusReward + earlySupporterBonus
        };
    }

    public NftRewardBreakdown CalculateRoundTwoNftReward(string nftName, int mintOrder) 
    {
        TbcNft? nft = _nftService.GetNft(ROUND_TWO_POLICY_ID, nftName);
        int rank = int.Parse(nft?.RarityRank ?? "0");
        int baseReward = GetRoundTwoBaseReward(rank);
        decimal bonusReward = GetRoundTwoBonus(mintOrder);
        decimal earlySupporterBonus = AVAILABLE_REWARDS * ((decimal)1.0 / EFFECTIVE_NFTS);

        return new() 
        {
            BaseReward = baseReward,
            BonusReward = bonusReward,
            EarlySupporterBonus = earlySupporterBonus,
            TotalReward = baseReward + bonusReward + earlySupporterBonus
        };
    }

    private int GetRoundOneBaseReward(int rank) 
    {
        return rank switch 
        {
            (<= 0)            => 0,
            (>= 1) and (< 16) => 28_000,
            (< 101)           => 17_500,
            (< 251)           => 12_600,
            (< 501)           => 11_200,
            (> 500)           => 10_500,
        }; 
    }

    private int GetRoundTwoBaseReward(int rank) 
    {
        return rank switch 
        {
            (<= 0)              => 0,
            (>= 1) and (< 24)   => 7_000,
            (< 222)             => 6_000,
            (< 665)             => 5_000,
            (< 1328)            => 4_500,
            (> 1327)            => 4_200
        }; 
    }

    private int GetRoundTwoBonus(int mintOrder)
    {
        return mintOrder switch
        {
            (<= 0)                    => 0,
            (>= 1) and (<= 1_200)     => 10_733,
            (<= 1_400)                => 9_733,
            (<= 1_600)                => 8_400,
            (<= 1_800)                => 6_400,
            (>= 1_800) and (<= 2_000) => 8_400,
            (> 2_000)                 => 4_000
        };
    }
}
