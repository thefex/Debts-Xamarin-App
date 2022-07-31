using Debts.iOS.ViewControllers.Menu;
using Debts.ViewModel.Finances;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Config.Presenter
{
    public class FilterFinancesByStateDateCustomPresenter : ICustomPresenter
    {
        private UIViewController ViewController;
        public bool HandleShowRequest(MvxViewModelRequest request)
        {
            var viewModelRequest = request as MvxViewModelInstanceRequest;
            ViewController = FilterFinancesByStateRangeViewController.Show(viewModelRequest.ViewModelInstance as FilterFinancesByStateViewModel);
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
            return request.ViewModelType == typeof(FilterFinancesByStateViewModel);
        }

        public bool ShouldHandleRequest(IMvxViewModel viewModel)
        {
            return viewModel.GetType() == typeof(FilterFinancesByStateViewModel);
        }
    }
}