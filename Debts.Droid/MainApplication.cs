using System;
using Android.App;
using Android.Gms.Ads;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Graphics.Drawable;
using Android.Support.V7.App;
using Debts.Droid.Config;
using Debts.Droid.Services;
using Debts.Droid.Services.AppGrowth;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;

namespace Debts.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }
        
        
        public override void OnCreate()
        {
            base.OnCreate();

            AppCompatDelegate.CompatVectorFromResourcesEnabled = true;
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this);
            
            AppCenter.Start(AndroidAppConstants.AppCenterId, typeof(Crashes));
            MobileAds.Initialize(this);
            
            
            IncrementAppRunCounter();
        }

        private void IncrementAppRunCounter()
        {
            new AndroidAppRelatedStatisticsService(new SharedPreferencesStorageService(), this)
                .IncrementAppRunCounter();
        }
    }
}