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
    public class FilterByBudgetCategorySubPresenter : MvxSubpresenter
    {
        public FilterByBudgetCategorySubPresenter(MvxAppPresenter appPresenter) : base(appPresenter)
        {
        }

        public override Task<bool> HandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity;

            var fragmentToShow = new FilterBudgetByCategoryFragment()
            {
                ViewModel = (request as MvxViewModelInstanceRequest).ViewModelInstance as FilterBudgetByCategoryViewModel
            };
            fragmentToShow.Show(currentActivity.SupportFragmentManager, FilterBudgetByCategoryFragment.FilterBudgetByCategoryTag);
            return Task.FromResult(true);
        }

        public override bool ShouldHandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            return request.ViewModelType == typeof(FilterBudgetByCategoryViewModel);
        }

        public override bool ShouldHandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            return viewModel.GetType() == typeof(FilterBudgetByCategoryViewModel);
        }

        public override Task<bool> HandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity;
            var fragment = currentActivity.SupportFragmentManager.FindFragmentByTag(FilterBudgetByCategoryFragment
                .FilterBudgetByCategoryTag) as FilterBudgetByCategoryFragment;
            
            fragment.Dismiss();
            return Task.FromResult(true);
        }
    }
}