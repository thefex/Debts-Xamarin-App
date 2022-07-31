using Debts.iOS.ViewControllers.Base;
using Debts.ViewModel.Statistics;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Config.ViewModelResolver
{
    class FilterStatisticsByDateViewModelResolver : IMvxCustomViewModelResolver
    {
        public IMvxViewModel GetViewModel()
        {
            var mainViewController = UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;
            return new FilterStatisticsByDateViewModel((mainViewController.GetLastPage() as MvxViewController).ViewModel as StatisticsViewModel);
        }
    }
}