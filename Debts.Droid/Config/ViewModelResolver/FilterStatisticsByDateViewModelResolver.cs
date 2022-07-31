using System.Linq;
using Debts.ViewModel.Statistics;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.ViewModelResolver
{
    class FilterStatisticsByDateViewModelResolver : MvxCustomViewModelResolver
    {
        protected override IMvxViewModel GetViewModelFromCurrentActivity(MvxAppCompatActivity currentActivity)
        {
            var statisticsViewModel = (currentActivity.SupportFragmentManager.Fragments.Last(x => x is MvxFragment) as MvxFragment).ViewModel as StatisticsViewModel;
            return new FilterStatisticsByDateViewModel(statisticsViewModel);
        }
    }
}