using System;
using System.Threading.Tasks;
using Debts.Droid.Activities;
using Debts.Droid.Fragments.Finances;
using Debts.ViewModel.Finances;
using MvvmCross;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.Presenter
{
    public class FilterByFinancesDateSubPresenter : MvxSubpresenter
    {
        public FilterByFinancesDateSubPresenter(MvxAppPresenter appPresenter) : base(appPresenter)
        {
        }

        public override Task<bool> HandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity;

            var fragmentToShow = new FilterFinancesByDateFragment()
            {
                ViewModel = (request as MvxViewModelInstanceRequest).ViewModelInstance as FilterFinancesByDateViewModel
            };
            fragmentToShow.Show(currentActivity.SupportFragmentManager, FilterFinancesByDateFragment.FilterFinancesByDateFragmentTag);
            return Task.FromResult(true);
        }

        public override bool ShouldHandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            return request.ViewModelType == typeof(FilterFinancesByDateViewModel);
        }

        public override bool ShouldHandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            return viewModel.GetType() == typeof(FilterFinancesByDateViewModel);
        }

        public override Task<bool> HandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity;
            var fragment = currentActivity.SupportFragmentManager.FindFragmentByTag(FilterFinancesByDateFragment
                .FilterFinancesByDateFragmentTag) as FilterFinancesByDateFragment;
            
            fragment.Dismiss();
            return Task.FromResult(true);
        }
    }
}