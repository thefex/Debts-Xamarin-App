using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.BottomAppBar;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Transitions;
using Android.Views;
using Android.Widget;
using Debts.Droid.Activities.Transitions;
using Debts.Droid.Core.Extensions;
using Debts.Droid.Core.ViewControllers;
using Debts.Droid.Fragments;
using Debts.Droid.Fragments.Budget;
using Debts.Droid.Fragments.Contacts;
using Debts.Droid.Fragments.Finances;
using Debts.Droid.Fragments.Main;
using Debts.Droid.Messenging.Observers;
using Debts.Droid.Services.Walkthrough;
using Debts.Messenging;
using Debts.Services;
using Debts.Services.Settings;
using Debts.ViewModel;
using Debts.ViewModel.FinancesViewModel;
using Java.Lang.Reflect;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.Preference;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Presenters.Attributes;
using MvvmCross.ViewModels;
using Fragment = Android.Support.V4.App.Fragment;

namespace Debts.Droid.Activities
{
    [MvxActivityPresentation(ViewModelType = typeof(MainViewModel), ViewType = typeof(MainActivity))]
    [Activity(MainLauncher = false, Theme = "@style/BottomAppBarRelatedTheme", ClearTaskOnLaunch = true, LaunchMode = LaunchMode.SingleTask, WindowSoftInputMode = SoftInput.AdjustPan,
        ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new []{ Android.Content.Intent.ActionView}, 
        Categories = new [] { Android.Content.Intent.CategoryBrowsable, Android.Content.Intent.CategoryDefault},
        DataHost = "debtsmanager.page.link",
        DataScheme = "https"
        )]
    public class MainActivity : BaseApplicationMvxActivity<MainViewModel, string>, IMvxAndroidSharedElements
    {
        private CustomSnackController _customSnackController;

        public MainActivity()
        {
            SubViewTransitionInvoker = new SubViewTransitionInvoker(() => ViewModel);
        }

        public MainActivity(IntPtr ptr, JniHandleOwnership ownership) : base(ptr, ownership)
        {
        }

        protected override IEnumerable<IMvxMessageObserver> GetMessageObservers()
        {
            var observers = base.GetMessageObservers().ToList();
            
            observers.Add(new CustomToastMessageObserver(() => _customSnackController));
            return observers;
        }

        protected override void OnCreateView(Bundle bundle)
        {
            base.OnCreateView(bundle);
            
            _customSnackController = new CustomSnackController();
            _customSnackController.Initialize(FindViewById<ViewGroup>(Resource.Id.custom_snackbar_container));
            var bottomAppBar = FindViewById<BottomAppBar>(Resource.Id.bottomAppBar);
            FloatingActionButton  = FindViewById<FloatingActionButton>(Resource.Id.fab);
            SetSupportActionBar(bottomAppBar);

            SubViewTransitionInvoker.Initialize(bottomAppBar, FloatingActionButton);
            ViewModel.Budget.Execute();

            bottomAppBar.Post(() =>
            {
                bottomAppBar.ReplaceMenu(Resource.Menu.budget_menu);
            });

            BudgetWalkthroughService budgetWalkthroughService = new BudgetWalkthroughService(
                Mvx.IoCProvider.Resolve<WalkthroughService>()
            );

            bottomAppBar.PostDelayed(() =>
            {
                budgetWalkthroughService.Initialize(FloatingActionButton, this);
                budgetWalkthroughService.ShowIfPossible(this);
            }, 375);
            
            bottomAppBar.PostDelayed(() =>
            {
                Intent?.TryHandleMainActivityIntentData(this); 
            }, 425);
        }
         
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            intent?.TryHandleMainActivityIntentData(this);
        } 
        
        public FloatingActionButton FloatingActionButton { get; private set; }
 
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var lastFragment = SupportFragmentManager.Fragments
                .LastOrDefault(x => x is MvxFragment || x is MvxDialogFragment);

            if (lastFragment is FinanceDetailsFragment ||
                lastFragment is ContactDetailsFragment ||
                lastFragment is BudgetDetailsFragment)
                return lastFragment.OnOptionsItemSelected(item);
            
            switch (item.ItemId)
            { 
                case Android.Resource.Id.Home:  
                    var menu = new MainBottomMenuFragment()
                    {
                        ViewModel = ViewModel,
                    }; 
                    menu.Show(SupportFragmentManager, menu.Tag);
                    return true; 
            }

            if (lastFragment is AllFinancesFragment ||
                lastFragment is MyDebtsFragment ||
                lastFragment is MyLoansFragment ||
                lastFragment is FavoritesFinancesFragment ||
                lastFragment is BudgetFragment ||
                lastFragment is StatisticsFragment ||
                lastFragment is ContactListFragment)
                return lastFragment.OnOptionsItemSelected(item);
            
            return base.OnOptionsItemSelected(item);
        }
        

        public override void OnBackPressed()
        {
            var lastFragment = SupportFragmentManager.Fragments.LastOrDefault();
            var lastMvxFragment = SupportFragmentManager.Fragments.LastOrDefault(x => x is MvxFragment || x is MvxDialogFragment || x is MvxPreferenceFragmentCompat);
 
            if (lastFragment != null && lastMvxFragment != null && lastFragment != lastMvxFragment)
            {
                if (InvokeLastFragmentAction(lastMvxFragment))
                    return;
            }

            if (InvokeLastFragmentAction(lastFragment)) 
                return;
            
            base.OnBackPressed();
        }

        private static bool InvokeLastFragmentAction(Fragment lastFragment)
        {
            if (lastFragment is FinanceDetailsFragment financeDetailsFragment)
            {
                financeDetailsFragment.ViewModel.Close.Execute();
                return true;
            }

            if (lastFragment is ContactDetailsFragment contactDetailsFragment)
            {
                contactDetailsFragment.ViewModel.Close.Execute();
                return true;
            }

            if (lastFragment is BudgetDetailsFragment budgetDetailsFragment)
            {
                budgetDetailsFragment.ViewModel.Close.Execute();
                return true;
            }

            if (lastFragment is AddFinanceOperationFragment addFinanceOperationFragment)
            {
                addFinanceOperationFragment.ViewModel.Close.Execute();
                return true;
            }

            if (lastFragment is AddBudgetItemFragment addBudgetItemFragment)
            {
                addBudgetItemFragment.ViewModel.Close.Execute();
                return true;
            }

            if (lastFragment is AddContactFragment addContactFragment)
            {
                addContactFragment.ViewModel.Close.Execute();
                return true;
            }

            if (lastFragment is PickContactFragment pickContactFragment)
            {
                pickContactFragment.ViewModel.Close.Execute();
                return true;
            }

            if (lastFragment is GoPremiumFragment goPremiumFragment)
            {
                goPremiumFragment.ViewModel.Skip.Execute();
                return true;
            }

            if (lastFragment is RateAppFragment rateAppFragment)
            {
                rateAppFragment.ViewModel.Skip.Execute();
                return true;
            }

            return false;
        }


        public override int LayoutId => Resource.Layout.activity_main;
 
        public SubViewTransitionInvoker SubViewTransitionInvoker { get; private set; }
        
        public IDictionary<string, View> FetchSharedElementsToAnimate(MvxBasePresentationAttribute attribute, MvxViewModelRequest request)
        {
            Dictionary<string, View> sharedElements = new Dictionary<string, View>();

            if (request.ViewModelType == typeof(FinanceDetailsViewModel))
            {
                if (SupportFragmentManager.Fragments?.LastOrDefault() is IFinanceListView financeListView)
                {
                    var titleText = financeListView.GetSelectedTitleTextView();
                    if (titleText != null)
                        sharedElements.Add(nameof(Resource.Id.text_title), titleText);

                    var avatarView = financeListView.GetAvatarImageView();
                    if (avatarView != null)
                        sharedElements.Add(nameof(Resource.Id.avatarView), avatarView);

                    var view = financeListView.GetRootView();
                    if (view != null)
                        sharedElements.Add(nameof(Resource.Id.root_view), view);
                }
            }

            return sharedElements;
        }
    }
}