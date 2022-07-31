using System;
using System.Linq;
using Debts.iOS.ViewControllers.Menu;
using MaterialComponents;
using UIKit;

namespace Debts.iOS.ViewControllers.Transition
{
    class FinanceListSubViewTransitionAction : ITransitionSubViewAction
    {
        private readonly Action _filterByDate;
        private readonly Action _filterByStatus;

        public FinanceListSubViewTransitionAction(Action filterByDate, Action filterByStatus)
        {
            _filterByDate = filterByDate;
            _filterByStatus = filterByStatus;
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
                    _filterByDate();
                });

            var filterStatusMenuItem = new UIBarButtonItem(UIImage.FromBundle("filter_status"),
                UIBarButtonItemStyle.Plain,
                (e, a) =>
                {
                    _filterByStatus();
                });
            
            appBar.LeadingBarButtonItems = new UIBarButtonItem[] { menuItem };
            appBar.TrailingBarButtonItems = new UIBarButtonItem[]
            {
                filterStatusMenuItem, filterDateMenuItem
            };
        }
    }
}