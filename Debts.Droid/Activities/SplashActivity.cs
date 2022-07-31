using Android.App;
using Android.Content.PM;
using Android.Runtime;
using MvvmCross.Platforms.Android.Views;

namespace Debts.Droid.Activities
{
    [Register("com.debts.droid.activities.SplashActivity")]
    [Activity(MainLauncher = true
        , Theme = "@style/splashTheme"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : MvxSplashScreenActivity
    {
        public SplashActivity() : base(Resource.Layout.SplashLayout)
        {
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);
        }
    }
}