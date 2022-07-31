using Debts.iOS.ViewControllers.Menu;
using Debts.ViewModel.Budget;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Config.Presenter
{
    public class FilterBudgetByCategoryCustomPresenter : ICustomPresenter
    {
        private UIViewController ViewController;
        public bool HandleShowRequest(MvxViewModelRequest request)
        {
            var viewModelRequest = request as MvxViewModelInstanceRequest;
            ViewController = FilterBudgetByCategoryViewController.Show(viewModelRequest.ViewModelInstance as FilterBudgetByCategoryViewModel);
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
            return request.ViewModelType == typeof(FilterBudgetByCategoryViewModel);
        }

        public bool ShouldHandleRequest(IMvxViewModel viewModel)
        {
            return viewModel.GetType() == typeof(FilterBudgetByCategoryViewModel);
        }
    }
}