using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.Data;
using Debts.Droid.Services.Walkthrough;
using Debts.iOS.Config;
using Debts.iOS.Core.Messenging;
using Debts.iOS.Services.Notifications;
using Debts.iOS.Services.Walkthrough;
using Debts.iOS.ViewControllers.Budget;
using Debts.iOS.ViewControllers.Contacts;
using Debts.iOS.ViewControllers.Finances;
using Debts.iOS.ViewControllers.Menu;
using Debts.iOS.ViewControllers.Transition;
using Debts.iOS.ViewModels;
using Debts.Messenging;
using Debts.Services;
using Debts.Services.Settings;
using Debts.ViewModel;
using Foundation;
using MaterialComponents;
using MessageUI;
using MvvmCross;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.ViewControllers.Base
{
    [MvxRootPresentation(WrapInNavigationController = false)]
    public class MainViewController : BaseViewController<MainViewModel, string>, IMvxPageViewController
    {
        private UINavigationController navigationController;
        private SubViewTransitionInvoker _subViewTransitionInvoker;
        private FluentLayout bottomAppBarHeightConstraint;
        private CGSize measuredBottomAppBarSize;

        public MainViewController()
        {
        }

        public MainViewController(IntPtr handle) : base(handle)
        {
        }

        protected MainViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        public override void ViewSafeAreaInsetsDidChange()
        {
            base.ViewSafeAreaInsetsDidChange();
        }

        public void AdjustBottomAppBarHeight(UIEdgeInsets safeInsets)
        {
            bottomAppBarHeightConstraint.Constant = measuredBottomAppBarSize.Height + safeInsets.Bottom/2;
        }

        public BottomAppBarView BottomAppBarView { get; private set; }

        public override void PresentViewController(UIViewController viewControllerToPresent, bool animated, Action completionHandler)
        {
            if (viewControllerToPresent is MFMessageComposeViewController messageComposeViewController)
            {
                messageComposeViewController.Finished += (e, a) =>
                {
                    if (GetLastPage() is FinanceDetailsViewController financeDetailsViewController)
                    {
                        financeDetailsViewController.ViewModel.HandleShareOrSmsNote();
                    }
                };
            }

            if (viewControllerToPresent is UIActivityViewController viewController)
            {
                viewController.CompletionWithItemsHandler += (type, isCompleted, e, a) =>
                {
                    if (isCompleted && GetLastPage() is FinanceDetailsViewController financeDetailsViewController)
                        financeDetailsViewController.ViewModel.HandleShareOrSmsNote();
                };
            }

            base.PresentViewController(viewControllerToPresent, animated, completionHandler);

        }


        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
 
            _subViewTransitionInvoker = new SubViewTransitionInvoker(() => ViewModel);
            
            navigationController = new UINavigationController();
            navigationController.ToolbarHidden = true;
            navigationController.View.BackgroundColor = AppColors.GrayBackground;
            navigationController.NavigationBarHidden = true;
            navigationController.TransitioningDelegate = new MaskedTransitionController();

            View.BackgroundColor = AppColors.GrayBackground;

            var bottomAppBar = BuildBottomAppBarView();
            _subViewTransitionInvoker.AppBar = bottomAppBar;
            BottomAppBarView = bottomAppBar;
            
            Add(navigationController.View);
            Add(bottomAppBar);
            measuredBottomAppBarSize = bottomAppBar.SizeThatFits(View.Frame.Size);
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            View.AddConstraints(
                    navigationController.View.AtTopOf(View),
                    navigationController.View.AtLeftOf(View),
                    navigationController.View.AtRightOf(View),
                    navigationController.View.AtBottomOf(View),
                
                    bottomAppBar.AtLeftOf(View),
                    bottomAppBar.AtRightOf(View),
                    bottomAppBar.AtBottomOf(View),
                    bottomAppBarHeightConstraint = bottomAppBar.Height().EqualTo(measuredBottomAppBarSize.Height)
                );

            if (UIApplication.SharedApplication.KeyWindow != null)
                AdjustBottomAppBarHeight(UIApplication.SharedApplication.KeyWindow.SafeAreaInsets);
            
            ViewModel.Budget.Execute();

            bottomAppBar.FloatingButton.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ViewModel.Add.Execute();
            }));
            BudgetWalkthroughService mainWalkthroughService = new BudgetWalkthroughService(Mvx.IoCProvider.Resolve<WalkthroughService>());
            await Task.Delay(500);
 
            mainWalkthroughService.Initialize(bottomAppBar, this);
            mainWalkthroughService.ShowIfPossible(this);  
        }

        BottomAppBarView BuildBottomAppBarView()
        {
            BottomAppBarView bottomAppBarView = new BottomAppBarView();
            bottomAppBarView.BarTintColor = AppColors.Primary;
            bottomAppBarView.FloatingButton.BackgroundColor = AppColors.Accent;  
            bottomAppBarView.LeadingBarItemsTintColor = UIColor.White;
            bottomAppBarView.TrailingBarItemsTintColor = UIColor.White;
            bottomAppBarView.FloatingButton.UserInteractionEnabled = true;

            return bottomAppBarView;
        }
         
        public void AddPage(UIViewController viewController, MvxPagePresentationAttribute attribute)
        {
            bool animate = navigationController.ViewControllers.Any();
            if (IsMainViewRelated(viewController))
                navigationController.ViewControllers = new UIViewController[] {};
            
            navigationController.PushViewController(viewController, animate);
            OnSubViewAppeared(viewController);
        }

        bool IsMainViewRelated(UIViewController viewController)
        {
            return viewController is AllFinancesViewController ||
                   viewController is BudgetViewController ||
                   viewController is DebtsFinancesViewController ||
                   viewController is LoansFinancesViewController ||
                   viewController is FavoritesFinancesViewController ||
                   viewController is ContactsViewController ||
                   viewController is SettingsViewController ||
                   viewController is StatisticsViewController;
        }
        

        public void OnSubViewAppeared(UIViewController viewController)
        {
            _subViewTransitionInvoker.OnSubViewAppeared(viewController);   
        }

        public bool RemovePage(IMvxViewModel viewModel)
        {
            var uiViewControllers = navigationController.ViewControllers.ToList();
            var viewControllerForViewModel = uiViewControllers.FirstOrDefault(x =>
                x is MvxViewController mvxViewController && mvxViewController.ViewModel == viewModel);

            if (viewControllerForViewModel != null)
            {
                if (viewControllerForViewModel == uiViewControllers.Last())
                {
                    navigationController.PopViewController(true);
                }
                else
                {
                    uiViewControllers.Remove(viewControllerForViewModel);
                    navigationController.ViewControllers = uiViewControllers.ToArray();
                }
            }

            
            var lastViewController = navigationController.ViewControllers.LastOrDefault(x => x is MvxViewController);
            
            if (lastViewController != null)
                _subViewTransitionInvoker.OnSubViewAppeared(lastViewController);
            
            return true;
        }

        public UIViewController GetLastPage() => navigationController.ViewControllers.LastOrDefault();


        protected override IEnumerable<IMvxMessageObserver> GetMessageObservers()
        {
            foreach (var observer in base.GetMessageObservers())
                yield return observer;

            yield return new ToastMvxObserver(() => View);
            yield return new MessageDialogMessageObserver(this);
            yield return new QuestionMessageDialogMessageObserver(this);
            yield return new CheckboxQuestionMessageDialogMessageObserver(this);
        }
    }
}