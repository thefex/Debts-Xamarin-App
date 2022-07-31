using System;
using System.Collections.Generic;
using Debts.Droid.Services.Walkthrough;
using Debts.Resources;
using Debts.Services.Settings;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using SpotlightXamarin.IOS;
using UIKit;

namespace Debts.iOS.Services.Walkthrough
{
    public class BudgetWalkthroughService : PlatformWalkthroughService
    {
        private readonly WalkthroughService _walkthroughService;
        private UIView addView;
        private UIView menuView;
        private UIView dateView;
        private UIView filterView;

        public BudgetWalkthroughService(WalkthroughService walkthroughService)
        {
            _walkthroughService = walkthroughService;
        }

        public void Initialize(BottomAppBarView bottomAppBarView, UIViewController activity)
        {
            addView = bottomAppBarView.FloatingButton;
            menuView = bottomAppBarView.Subviews[2].Subviews[1];
            dateView = bottomAppBarView.Subviews[2].Subviews[2].Subviews[1];
            filterView = bottomAppBarView.Subviews[2].Subviews[2].Subviews[0];
        }
        public void ShowIfPossible(UIViewController viewController)
        {
            try
            {
                if (_walkthroughService.IsBudgetListTutorialShown())
                    return;
                
                _walkthroughService.SetBudgetListTutorialAsShown();

                
                var menuTargetBuilder = new SimpleTargetBuilder(viewController)
                    .SetTitle(TextResources.WalkthroughService_Main_Menu_Title)
                    .SetDescription(TextResources.WalkthroughService_Main_Menu_Content)
                    .SetPoint(GetRelativeCenterPosition(menuView, viewController))
                    .SetRadius((int)menuView.Frame.Width);
                
                var addTargetBuilder = new SimpleTargetBuilder(viewController)
                    .SetTitle(TextResources.WalkthroughService_Budget_AddOperation_Title)
                    .SetDescription(TextResources.WalkthroughService_Budget_AddOperation_Content)
                    .SetPoint(GetRelativeCenterPosition(addView, viewController))
                    .SetRadius((int) addView.Frame.Width);

                var dateTargetBuilder = new SimpleTargetBuilder(viewController)
                    .SetTitle(TextResources.WalkthroughService_Budget_FilterByDate_Title)
                    .SetDescription(TextResources.WalkthroughService_Budget_FilterByDate_Content)
                    .SetPoint(GetRelativeCenterPosition(dateView, viewController))
                    .SetRadius((int)dateView.Frame.Width);
                
                var filterTargetBuilder = new SimpleTargetBuilder(viewController)
                    .SetTitle(TextResources.WalkthroughService_Budget_FilterByCategory_Title)
                    .SetDescription(TextResources.WalkthroughService_Budget_FilterByCategory_Content)
                    .SetPoint(GetRelativeCenterPosition(filterView, viewController))
                    .SetRadius((int)dateView.Frame.Width);

                
                List<Target> targetList = new List<Target>()
                {
                    menuTargetBuilder.Build(),
                    addTargetBuilder.Build(),
                    dateTargetBuilder.Build(),
                    filterTargetBuilder.Build()
                };
 
                var spotlight = new Spotlight(viewController, targetList, 450);
                spotlight.Start();
            }
            catch (Exception)
            {
                
            }
        }
    }
}