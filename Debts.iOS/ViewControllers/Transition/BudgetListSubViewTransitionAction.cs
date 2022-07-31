using System;
using Debts.iOS.ViewControllers.Menu;
using Debts.ViewModel.Budget;
using MaterialComponents;
using UIKit;

namespace Debts.iOS.ViewControllers.Transition
{
    class BudgetListSubViewTransitionAction : ITransitionSubViewAction
    {
        private readonly Func<BudgetListViewModel> _viewModel;

        public BudgetListSubViewTransitionAction(Func<BudgetListViewModel> viewModel)
        {
            _viewModel = viewModel;
        }

        public void OnSubViewAppearedAction(BottomAppBarView appBar)
        {
            appBar.SetFloatingButtonHidden(false, true);
            appBar.FloatingButtonPosition = BottomAppBarFloatingButtonPosition.Center;
            appBar.FloatingButton.SetImage(UIImage.FromBundle("plus"), UIControlState.Normal);
            appBar.FloatingButton.SetImageTintColor(UIColor.White, UIControlState.Normal);
            
            var menuItem = new UIBarButtonItem(UIImage.FromBundle("menu_hamburger"), UIBarButtonItemStyle.Plain, (e, a) =>
            {
                MenuViewController.Show();
            });

            var filterDateMenuItem = new UIBarButtonItem(UIImage.FromBundle("filter_date"),
                UIBarButtonItemStyle.Plain,
                (e, a) =>
                {
                    _viewModel().FilterByDate.Execute();
                });

            var filterStatusMenuItem = new UIBarButtonItem(UIImage.FromBundle("filter_status"),
                UIBarButtonItemStyle.Plain,
                (e, a) => { _viewModel().FilterByStatus.Execute(); });
            
            appBar.LeadingBarButtonItems = new UIBarButtonItem[] { menuItem };
            appBar.TrailingBarButtonItems = new UIBarButtonItem[]
            {
                filterStatusMenuItem, filterDateMenuItem
            };
        }
    }
}