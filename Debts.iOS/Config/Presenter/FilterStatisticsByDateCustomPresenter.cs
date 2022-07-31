using Debts.iOS.ViewControllers.Menu;
using Debts.ViewModel.Statistics;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Config.Presenter
{
    public class FilterStatisticsByDateCustomPresenter : ICustomPresenter
    {
        private UIViewController ViewController;
        public bool HandleShowRequest(MvxViewModelRequest request)
        {
            var viewModelRequest = request as MvxViewModelInstanceRequest;
            ViewController = FilterStatisticsByDateRangeViewController.Show(viewModelRequest.ViewModelInstance as FilterStatisticsByDateViewModel);
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
            return request.ViewModelType == typeof(FilterStatisticsByDateViewModel);
        }

        public bool ShouldHandleRequest(IMvxViewModel viewModel)
        {
            return viewModel.GetType() == typeof(FilterStatisticsByDateViewModel);
        }
    }
}