using System.Threading.Tasks;

namespace Debts.Services.AppGrowth
{
    public abstract class AdvertisementService
    {
        private readonly PremiumService _premiumService;

        public AdvertisementService(PremiumService premiumService)
        {
            _premiumService = premiumService;
        }

        public bool AreAdsAvailable => _premiumService.AreAdsAvailable;

        public bool IsRewardAdAvailable => _premiumService.CanExtendPremiumByOneDay();

        public abstract void PreloadFullScreenAd();

        public abstract Task ShowFullScreenAd();

        public abstract void PreloadRewardedAd();
        public abstract Task<bool> ShowFullScreenRewardAd();

        public abstract bool IsRewardAdLoaded { get; }
    }
}