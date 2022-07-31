using System;
using System.Threading.Tasks;
using Debts.iOS.ViewControllers.Menu;
using Debts.ViewModel.Statistics;
using MaterialComponents;
using UIKit;

namespace Debts.iOS.ViewControllers.Transition
{
    class StatisticsSubViewTransitionAction : ITransitionSubViewAction
    {
        private readonly Func<StatisticsViewModel> _statisticsViewModel;

        public StatisticsSubViewTransitionAction(Func<StatisticsViewModel> statisticsViewModel)
        {
            _statisticsViewModel = statisticsViewModel;
        }

        public async void OnSubViewAppearedAction(BottomAppBarView appBar)
        {
            appBar.SetFloatingButtonHidden(true, true);
            // bug workaround
            // when FAB is set to right - hidden menu items are not shown even though floating button is hidden
            appBar.FloatingButtonPosition = BottomAppBarFloatingButtonPosition.Center;

            var menuItem = new UIBarButtonItem(UIImage.FromBundle("menu_hamburger"), UIBarButtonItemStyle.Plain, (e, a) =>
            {
                MenuViewController.Show();
            });
            
            var filterDateMenuItem = new UIBarButtonItem(UIImage.FromBundle("filter_date"),
                UIBarButtonItemStyle.Plain,
                (e, a) =>
                {
                    _statisticsViewModel().FilterByDate.Execute();
                });

            appBar.LeadingBarButtonItems = new UIBarButtonItem[] { menuItem };
            appBar.TrailingBarButtonItems = new UIBarButtonItem[]
            {
                filterDateMenuItem
            };
        }
    }
}