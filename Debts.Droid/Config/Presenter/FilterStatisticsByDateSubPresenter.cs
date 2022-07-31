using System;
using System.Threading.Tasks;
using Debts.Droid.Activities;
using Debts.Droid.Fragments;
using Debts.ViewModel.Statistics;
using MvvmCross;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.Presenter
{
    public class FilterStatisticsByDateSubPresenter : MvxSubpresenter
    {
        public FilterStatisticsByDateSubPresenter(MvxAppPresenter appPresenter) : base(appPresenter)
        {
        }

        public override Task<bool> HandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity;

            var fragmentToShow = new FilterStatisticsByDateFragment()
            {
                ViewModel = (request as MvxViewModelInstanceRequest).ViewModelInstance as FilterStatisticsByDateViewModel
            };
            fragmentToShow.Show(currentActivity.SupportFragmentManager, FilterStatisticsByDateFragment.FragmentTag);
            return Task.FromResult(true);
        }

        public override bool ShouldHandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            return request.ViewModelType == typeof(FilterStatisticsByDateViewModel);
        }

        public override bool ShouldHandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            return viewModel.GetType() == typeof(FilterStatisticsByDateViewModel);
        }

        public override Task<bool> HandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MainActivity;
            var fragment = currentActivity.SupportFragmentManager.FindFragmentByTag(FilterStatisticsByDateFragment
                .FragmentTag) as FilterStatisticsByDateFragment;
            
            fragment.Dismiss();
            return Task.FromResult(true);
        }
    }
}