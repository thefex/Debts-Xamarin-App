using Debts.iOS.ViewControllers.Menu;
using Debts.ViewModel.Finances;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Config.Presenter
{
    public class FilterFinancesByDateCustomPresenter : ICustomPresenter
    {
        private UIViewController ViewController;
        public bool HandleShowRequest(MvxViewModelRequest request)
        {
            var viewModelRequest = request as MvxViewModelInstanceRequest;
            ViewController = FilterFinancesByDateRangeViewController.Show(viewModelRequest.ViewModelInstance as FilterFinancesByDateViewModel);
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
            return request.ViewModelType == typeof(FilterFinancesByDateViewModel);
        }

        public bool ShouldHandleRequest(IMvxViewModel viewModel)
        {
            return viewModel.GetType() == typeof(FilterFinancesByDateViewModel);
        }
    }
}