using System;
using System.Linq;
using System.Threading.Tasks;
using Debts.Droid.Activities;
using Debts.Droid.Fragments.Finances;
using Debts.ViewModel.Finances;
using MvvmCross;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.Presenter
{
    public class FilterByFinancesStateSubPresenter : MvxSubpresenter
    {
        public FilterByFinancesStateSubPresenter(MvxAppPresenter appPresenter) : base(appPresenter)
        {
        }

        public override Task<bool> HandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity;

            var fragmentToShow = new FilterFinancesByStateFragment()
            {
                ViewModel = (request as MvxViewModelInstanceRequest).ViewModelInstance as FilterFinancesByStateViewModel
            };
            fragmentToShow.Show(currentActivity.SupportFragmentManager, FilterFinancesByStateFragment.FilterFinancesByStateFragmentTag);
            return Task.FromResult(true);
        }

        public override bool ShouldHandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            return request.ViewModelType == typeof(FilterFinancesByStateViewModel);
        }

        public override bool ShouldHandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            return viewModel.GetType() == typeof(FilterFinancesByStateViewModel);
        }

        public override Task<bool> HandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity;
            var fragment = currentActivity.SupportFragmentManager.FindFragmentByTag(FilterFinancesByStateFragment
                .FilterFinancesByStateFragmentTag) as FilterFinancesByStateFragment;
            
            fragment.Dismiss();
            return Task.FromResult(true);
        }
    }
}