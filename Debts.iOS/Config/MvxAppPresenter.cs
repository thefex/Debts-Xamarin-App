using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Debts.iOS.Config.Presenter;
using Debts.iOS.ViewControllers.Base;
using Debts.iOS.ViewControllers.Budget;
using Debts.iOS.ViewControllers.Finances;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.Config
{
    public class MvxAppPresenter : MvxIosViewPresenter
    { 
        readonly IList<ICustomPresenter> _customPresenters = new List<ICustomPresenter>()
        {
            new FilterFinancesByDateCustomPresenter(),
            new FilterStatisticsByDateCustomPresenter(),
            new FilterFinancesByStateDateCustomPresenter(),
            new FilterBudgetByDateCustomPresenter(),
            new FilterBudgetByCategoryCustomPresenter(),
            new ImportContactsCustomPresenter(),
            new PickCategoryForFilterCustomPresenter()
        };
        
        public MvxAppPresenter(UIApplicationDelegate appDelegate, UIWindow window) : base(appDelegate, window)
        {
        }

        public override Task<bool> Show(MvxViewModelRequest request)
        {
            foreach (var item in _customPresenters)
            {
                if (item.ShouldHandleRequest(request))
                    return Task.FromResult(item.HandleShowRequest(request));
            }
            
            return base.Show(request);
        }
        
        

        public override Task<bool> Close(IMvxViewModel viewModel)
        {
            foreach (var item in _customPresenters)
            {
                if (item.ShouldHandleRequest(viewModel))
                    return Task.FromResult(item.HandleClose(viewModel));
            }
            
            return base.Close(viewModel);
        }

        protected override Task<bool> ShowModalViewController(UIViewController viewController, MvxModalPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            var mainViewController = (UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController);
            
            if (mainViewController != null)
                mainViewController.OnSubViewAppeared(viewController);
            
            return base.ShowModalViewController(viewController, attribute, request);
        }

        protected override async Task<bool> CloseModalViewController(IMvxViewModel toClose, MvxModalPresentationAttribute attribute)
        {
            var result = await base.CloseModalViewController(toClose, attribute);
            var mainViewController = UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;

            if (UIApplication.SharedApplication.KeyWindow.RootViewController?.ModalViewController is AddFinanceViewController addFinanceViewController)
                addFinanceViewController.ViewWillAppear(false);
            
            if (UIApplication.SharedApplication.KeyWindow.RootViewController?.ModalViewController is AddBudgetViewController addBudgetViewController)
                addBudgetViewController.ViewWillAppear(false);
            
            var presentedPage = mainViewController?.GetLastPage();
            
            if (presentedPage!=null)
                mainViewController.OnSubViewAppeared(presentedPage);

            return result;
        }
    }
}