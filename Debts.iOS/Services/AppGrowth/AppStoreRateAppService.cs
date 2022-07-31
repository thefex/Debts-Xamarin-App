using Debts.Services;
using Debts.Services.AppGrowth;
using MvvmCross.Navigation;
using StoreKit;

namespace Debts.iOS.Services.AppGrowth
{
    public class AppStoreRateAppService : RateAppService
    {
        public AppStoreRateAppService(AppRelatedStatisticsService appRelatedStatisticsService, IMvxNavigationService navigationService, IStorageService storageService) : base(appRelatedStatisticsService, navigationService, storageService)
        {
        }

        public override void LaunchAppRateProcess()
        {
            SKStoreReviewController.RequestReview();
        }
    }
}