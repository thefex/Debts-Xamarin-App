using Debts.iOS.ViewControllers.Contacts;
using Debts.ViewModel.Contacts;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Config.Presenter
{
    public class ImportContactsCustomPresenter : ICustomPresenter
    {
        private ImportContactsViewController controller;

        public bool HandleShowRequest(MvxViewModelRequest request)
        {
            var viewModel = (request as MvxViewModelInstanceRequest).ViewModelInstance;
            controller = new ImportContactsViewController {ViewModel = viewModel};
            
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
            return request.ViewModelType == typeof(ImportContactsViewModel);
        }

        public bool ShouldHandleRequest(IMvxViewModel viewModel)
        {
            return viewModel.GetType() == typeof(ImportContactsViewModel);
        }
    }
}