using Debts.iOS.ViewControllers.Base;
using Debts.ViewModel.Budget;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Config.ViewModelResolver
{
    class FilterBudgetCategoryViewModelResolver : IMvxCustomViewModelResolver
    {
        public IMvxViewModel GetViewModel()
        {
            var mainViewController = UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;
            return new FilterBudgetByCategoryViewModel((mainViewController.GetLastPage() as MvxViewController).ViewModel as BudgetListViewModel);
        }
    }
}