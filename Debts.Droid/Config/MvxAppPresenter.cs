using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Debts.Droid.Activities;
using Debts.Droid.Config.Presenter;
using Debts.Droid.Fragments;
using Debts.Droid.Fragments.Contacts;
using Debts.Model;
using Debts.ViewModel;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Support.V7.Preference;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace Debts.Droid.Config
{
    public class MvxAppPresenter : MvxAppCompatViewPresenter
    {
        readonly IList<MvxSubpresenter> _subpresenters;
        
        public MvxAppPresenter(IEnumerable<Assembly> androidViewAssemblies) : base(androidViewAssemblies)
        { 
            _subpresenters = new List<MvxSubpresenter>()
            { 
                new AddDialogFragmentSubPresenter(this), 
                new AddFinanceDetailsNoteSubPresenter(this),
                new FilterByFinancesDateSubPresenter(this),
                new FilterByFinancesStateSubPresenter(this),
                new FilterStatisticsByDateSubPresenter(this),
                new EmptyFragmentAnimationSubPresenter(this),
                new FilterByBudgetDateSubPresenter(this),
                new FilterByBudgetCategorySubPresenter(this)
            };
        }

        protected override Intent CreateIntentForRequest(MvxViewModelRequest request)
        {
            var intent = base.CreateIntentForRequest(request);

            if(request.ViewModelType == typeof(MainViewModel))
                intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);

            return intent;
        }

        protected override Task<bool> ShowFragment(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
        {
            foreach (var presenter in _subpresenters)
            {
                if (presenter.ShouldHandleShowFragmentRequest(view, attribute, request))
                    return presenter.HandleShowFragmentRequest(view, attribute, request);
            }

            return base.ShowFragment(view, attribute, request);
        }

        protected override Task<bool> CloseFragment(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            foreach (var presenter in _subpresenters)
            {
                if (presenter.ShouldHandleCloseFragmentRequest(viewModel, attribute))
                    return presenter.HandleCloseFragmentRequest(viewModel, attribute);
            }
            return base.CloseFragment(viewModel, attribute);
        }
        
        protected override void OnBeforeFragmentChanging(Android.Support.V4.App.FragmentTransaction ft, Android.Support.V4.App.Fragment fragment, MvxFragmentPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            base.OnBeforeFragmentChanging(ft, fragment, attribute, request);
            ft.SetReorderingAllowed(true);
        }

        protected override bool TryPerformCloseFragmentTransaction(Android.Support.V4.App.FragmentManager fragmentManager,
            MvxFragmentPresentationAttribute fragmentAttribute)
        {
            var lastFragment = fragmentManager.Fragments.LastOrDefault(IsMvxFragment);
            var result = base.TryPerformCloseFragmentTransaction(fragmentManager, fragmentAttribute);

            if (lastFragment!=null && (lastFragment is FinanceDetailsFragment || 
                                       lastFragment is ContactDetailsFragment ||
                                       lastFragment is BudgetDetailsFragment ||
                                       lastFragment is GoPremiumFragment ||
                                       lastFragment is RateAppFragment))
                OnFragmentPopped(null, lastFragment, fragmentAttribute);
            
            return result;
        }

        protected override void OnFragmentChanging(Android.Support.V4.App.FragmentTransaction ft, Android.Support.V4.App.Fragment fragment, MvxFragmentPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            base.OnFragmentChanging(ft, fragment, attribute, request);
            
        }

        protected override void OnFragmentPopped(Android.Support.V4.App.FragmentTransaction ft, Android.Support.V4.App.Fragment fragment, MvxFragmentPresentationAttribute attribute)
        {
            base.OnFragmentPopped(ft, fragment, attribute);

            if (fragment == null)
            {
                
            }
            
            if (Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity is MainActivity mainActivity && IsMvxFragment(fragment))
            {
                mainActivity.SubViewTransitionInvoker.OnSubViewDisappeared(mainActivity.SupportFragmentManager, fragment);
            }
        }

        bool IsMvxFragment(Fragment fragment)
        {
            return fragment is MvxFragment || 
                   fragment is MvxDialogFragment ||
                   fragment is MvxPreferenceFragmentCompat;
        }

        protected override void OnFragmentChanged(FragmentTransaction ft, Fragment fragment, MvxFragmentPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            base.OnFragmentChanged(ft, fragment, attribute, request);

            if (Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity is MainActivity mainActivity && IsMvxFragment(fragment))
            {
                mainActivity.SubViewTransitionInvoker.OnSubViewAppeared(fragment);
            }
                
        }

        public Task<bool> InvokeShowFragment(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
            => base.ShowFragment(view, attribute, request);

        public Task<bool> InvokeCloseFragment(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute) 
            => base.CloseFragment(viewModel, attribute);
    }
}