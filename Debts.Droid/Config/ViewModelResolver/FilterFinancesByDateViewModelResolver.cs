using System.Linq;
using Debts.ViewModel.Finances;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.ViewModels; 

namespace Debts.Droid.Config.ViewModelResolver
{
    class FilterFinancesByDateViewModelResolver : MvxCustomViewModelResolver
    {
        protected override IMvxViewModel GetViewModelFromCurrentActivity(MvxAppCompatActivity currentActivity)
        {
            var baseFinancesViewModel = (currentActivity.SupportFragmentManager.Fragments.Last(x => x is MvxFragment) as MvxFragment).ViewModel as BaseFinancesViewModel;
            return new FilterFinancesByDateViewModel(baseFinancesViewModel);
        }
    }
}