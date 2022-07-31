using System.Linq;
using Debts.ViewModel.Budget;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.ViewModelResolver
{
    class FilterBudgetByDateViewModelResolver : MvxCustomViewModelResolver
    {
        protected override IMvxViewModel GetViewModelFromCurrentActivity(MvxAppCompatActivity currentActivity)
        {
            var budgetViewModel =
                (currentActivity.SupportFragmentManager.Fragments.Last(x => x is MvxFragment) as MvxFragment).ViewModel
                as BudgetListViewModel;
            return new FilterBudgetByDateViewModel(budgetViewModel);
        }
    }
}