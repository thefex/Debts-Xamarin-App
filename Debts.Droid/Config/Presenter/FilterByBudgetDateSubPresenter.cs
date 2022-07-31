using System;
using System.Threading.Tasks;
using Debts.Droid.Activities;
using Debts.Droid.Fragments.Budget;
using Debts.ViewModel.Budget;
using MvvmCross;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.Presenter
{
    public class FilterByBudgetDateSubPresenter : MvxSubpresenter
    {
        public FilterByBudgetDateSubPresenter(MvxAppPresenter appPresenter) : base(appPresenter)
        {
        }

        public override Task<bool> HandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity;

            var fragmentToShow = new FilterBudgetByDateFragment()
            {
                ViewModel = (request as MvxViewModelInstanceRequest).ViewModelInstance as FilterBudgetByDateViewModel
            };
            fragmentToShow.Show(currentActivity.SupportFragmentManager, FilterBudgetByDateFragment.FilterBudgetByDateTag);
            return Task.FromResult(true);
        }

        public override bool ShouldHandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            return request.ViewModelType == typeof(FilterBudgetByDateViewModel);
        }

        public override bool ShouldHandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            return viewModel.GetType() == typeof(FilterBudgetByDateViewModel);
        }

        public override Task<bool> HandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity;
            var fragment = currentActivity.SupportFragmentManager.FindFragmentByTag(FilterBudgetByDateFragment
                .FilterBudgetByDateTag) as FilterBudgetByDateFragment;
            
            fragment.Dismiss();
            return Task.FromResult(true);
        }
    }
}