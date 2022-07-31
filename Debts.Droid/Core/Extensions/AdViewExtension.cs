using Android.Gms.Ads;
using Android.Views;

namespace Debts.Droid.Core.Extensions
{
    public static class AdViewExtension
    {
        public static void LoadAdOrHideIfRequired(this AdView adView, bool areAdsAvailable)
        {
            if (areAdsAvailable)
            {
                adView.Visibility = ViewStates.Visible;
                AdRequest adRequest = new AdRequest.Builder().Build();
                adView.LoadAd(adRequest);
            }
            else
            {
                adView.Visibility = ViewStates.Gone;
            }
        }
    }
}