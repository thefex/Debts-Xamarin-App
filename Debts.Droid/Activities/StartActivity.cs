using System;
using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Debts.Droid.Fragments.Onboard;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.OnboardViewModel;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Activities
{
    [MvxActivityPresentation(ViewModelType = typeof(StartViewModel), ViewType = typeof(StartActivity))]
    [Activity(MainLauncher = false, Theme = "@style/AppTheme", ClearTaskOnLaunch = true, LaunchMode = LaunchMode.SingleTask, ScreenOrientation = ScreenOrientation.Portrait)]
    public class StartActivity : BaseApplicationMvxActivity<StartViewModel, string>
    {
        public StartActivity()
        {
        }

        public StartActivity(IntPtr ptr, JniHandleOwnership ownership) : base(ptr, ownership)
        {
        }

        protected override void OnCreateView(Bundle bundle)
        {
            base.OnCreateView(bundle);
            
            var pager = FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.pager);
            if (pager != null)
            {


                var fragments = new List<MvxViewPagerOnboardFragmentAdapter.FragmentInfo>();

                for (int i = 0; i < ViewModel.OnboardPagedViewModels.Count; i++)
                {
                    var onboardViewModel = ViewModel.OnboardPagedViewModels[i] as OnboardViewModel;
                    fragments.Add(new MvxViewPagerOnboardFragmentAdapter.FragmentInfo()
                    {
                        Title = onboardViewModel.Title,
                        ViewModel = onboardViewModel,
                        Text = onboardViewModel.Description,
                        AnimationAssetName = "0" + (i+1).ToString() + "_onboard.json" 
                    });
                } 
                pager.Adapter = new MvxViewPagerOnboardFragmentAdapter(Application.Context, SupportFragmentManager, fragments);
            }

            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            tabLayout.SetupWithViewPager(pager);
        }

        public override int LayoutId => Resource.Layout.activity_onboard; 
    }
}