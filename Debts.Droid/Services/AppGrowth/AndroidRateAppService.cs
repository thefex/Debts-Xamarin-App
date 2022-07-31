using Android.Content;
using Android.Net;
using Debts.Services;
using Debts.Services.AppGrowth;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Android;

namespace Debts.Droid.Services.AppGrowth
{
    public class AndroidRateAppService : RateAppService
    {
        private readonly IMvxAndroidCurrentTopActivity _topActivity;

        public AndroidRateAppService(AppRelatedStatisticsService appRelatedStatisticsService, IMvxNavigationService navigationService, IStorageService storageService, IMvxAndroidCurrentTopActivity topActivity) : base(appRelatedStatisticsService, navigationService, storageService)
        {
            _topActivity = topActivity;
        }

        public override void LaunchAppRateProcess()
        {
            var context = _topActivity.Activity;
            
            Uri uri = Uri.Parse("market://details?id=" + context.PackageName);
            Intent goToMarket = new Intent(Intent.ActionView, uri);
            // To count with Play market backstack, After pressing back button, 
            // to taken back to our application, we need to add following flags to intent. 
            goToMarket.AddFlags(ActivityFlags.NoHistory |
                                ActivityFlags.NewDocument |
                                ActivityFlags.MultipleTask);
            try {
                context.StartActivity(goToMarket);
            } catch (ActivityNotFoundException e) {
                context.StartActivity(new Intent(Intent.ActionView,
                    Uri.Parse("http://play.google.com/store/apps/details?id=" + context.PackageName)));
            }
        }
    }
}