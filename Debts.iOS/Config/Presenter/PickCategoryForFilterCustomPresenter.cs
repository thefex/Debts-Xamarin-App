using Debts.iOS.ViewControllers.Budget;
using Debts.ViewModel.Budget;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Config.Presenter
{
    public class PickCategoryForFilterCustomPresenter : ICustomPresenter
    {
        private  PickBudgetCategoryForFilterViewController controller;

        public bool HandleShowRequest(MvxViewModelRequest request)
        {
            var viewModel = (request as MvxViewModelInstanceRequest).ViewModelInstance;
            controller = new PickBudgetCategoryForFilterViewController() {ViewModel = viewModel as PickBudgetCategoryForFilterViewModel};
            
            var keyWindowRootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            if (keyWindowRootViewController.ModalViewController != null && keyWindowRootViewController.PresentedViewController != null)
            {
                keyWindowRootViewController = keyWindowRootViewController.PresentedViewController;
                if (keyWindowRootViewController.ModalViewController != null && keyWindowRootViewController.PresentedViewController != null)
                    keyWindowRootViewController = keyWindowRootViewController.PresentedViewController;
            }

            keyWindowRootViewController.PresentModalViewController(
                controller, true 
            );
            

            return true;
        }

        public bool HandleClose(IMvxViewModel viewModel)
        {
            if (controller != null)
            {
                var keyWindowRootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                if (keyWindowRootViewController.ModalViewController != null && keyWindowRootViewController.PresentedViewController != null)
                {
                    keyWindowRootViewController = keyWindowRootViewController.PresentedViewController;
                    if (keyWindowRootViewController.ModalViewController != null && keyWindowRootViewController.PresentedViewController != null)
                        keyWindowRootViewController = keyWindowRootViewController.PresentedViewController;
                }

                keyWindowRootViewController.DismissModalViewController(true);
                controller = null;
            }
            return true;
        }

        public bool ShouldHandleRequest(MvxViewModelRequest request)
        {
            return request.ViewModelType == typeof(PickBudgetCategoryForFilterViewModel);
        }

        public bool ShouldHandleRequest(IMvxViewModel viewModel)
        {
            return viewModel.GetType() == typeof(PickBudgetCategoryForFilterViewModel);
        }
    }
}