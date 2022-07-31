using Debts.iOS.ViewControllers.Menu;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Finances;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Config.Presenter
{
    public class FilterBudgetByDateCustomPresenter : ICustomPresenter
    {
        private UIViewController ViewController;
        public bool HandleShowRequest(MvxViewModelRequest request)
        {
            var viewModelRequest = request as MvxViewModelInstanceRequest;
            ViewController = FilterBudgetByDateRangeViewController.Show(viewModelRequest.ViewModelInstance as FilterBudgetByDateViewModel);
            return true;
        }

        public bool HandleClose(IMvxViewModel viewModel)
        {
            ViewController?.ViewDidDisappear(true);
            ViewController?.DismissViewController(true, () =>
            {
                
            });
            ViewController = null;
            return true;
        }

        public bool ShouldHandleRequest(MvxViewModelRequest request)
        {
            return request.ViewModelType == typeof(FilterBudgetByDateViewModel);
        }

        public bool ShouldHandleRequest(IMvxViewModel viewModel)
        {
            return viewModel.GetType() == typeof(FilterBudgetByDateViewModel);
        }
    }
}