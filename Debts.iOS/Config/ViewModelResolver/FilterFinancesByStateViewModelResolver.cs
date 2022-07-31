using System.Linq;
using Debts.iOS.ViewControllers.Base;
using Debts.ViewModel.Finances;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Config.ViewModelResolver
{
    class FilterFinancesByStateViewModelResolver : IMvxCustomViewModelResolver
    {
        public IMvxViewModel GetViewModel()
        {
            var mainViewController = UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;
            return new FilterFinancesByStateViewModel((mainViewController.GetLastPage() as MvxViewController).ViewModel as BaseFinancesViewModel);
        }
    }
}