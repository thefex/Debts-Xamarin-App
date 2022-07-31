using System;
using System.Collections.Generic;
using System.Linq;
using Debts.Model.Walkthrough;
using Debts.Resources;
using Debts.Services.Settings;
using MaterialComponents;
using SpotlightXamarin.IOS;
using UIKit;

namespace Debts.Droid.Services.Walkthrough
{
    public class MainWalkthroughService : PlatformWalkthroughService
    {
        private readonly WalkthroughService _walkthroughService;
        private UIView addView;
        private UIView dateView;
        private UIView filterView;

        public MainWalkthroughService(WalkthroughService walkthroughService)
        {
            _walkthroughService = walkthroughService;
        }

        public void Initialize(BottomAppBarView bottomAppBarView, UIViewController activity)
        {
            addView = bottomAppBarView.FloatingButton;
            dateView = bottomAppBarView.Subviews[2].Subviews[2].Subviews[1];
            filterView = bottomAppBarView.Subviews[2].Subviews[2].Subviews[0];
        }
        
        public void ShowIfPossible(UIViewController activity)
        {
            try
            {
                var mainViewController = GetMainViewController();
                Initialize(mainViewController.BottomAppBarView, mainViewController);
                
                var typesToShow = _walkthroughService.GetWalkthroughTypesToShowForMainView();
                var addTargetBuilder = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Main_AddOperation_Title)
                    .SetDescription(TextResources.WalkthroughService_Main_AddOperation_Content)
                    .SetPoint(GetRelativeCenterPosition(addView, activity))
                    .SetRadius((int)addView.Frame.Width);
 
                var dateTargetBuilder = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Main_FilterByDate_Title)
                    .SetDescription(TextResources.WalkthroughService_Main_FilterByDate_Content)
                    .SetPoint(GetRelativeCenterPosition(dateView, activity))
                    .SetRadius((int)dateView.Frame.Width);

                var filterByTypeBuilder = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Main_FilterByStatus_Title)
                    .SetDescription(TextResources.WalkthroughService_Main_FilterByStatus_Content)
                    .SetPoint(GetRelativeCenterPosition(filterView, activity))
                    .SetRadius((int)filterView.Frame.Width);

                List<Target> targetList = new List<Target>();

                if (typesToShow.Contains(MainWalkthroughType.Add))
                    targetList.Add(addTargetBuilder.Build()); 
                if (typesToShow.Contains(MainWalkthroughType.FilterByDates))
                    targetList.Add(dateTargetBuilder.Build());
                if (typesToShow.Contains(MainWalkthroughType.FilterByType))
                    targetList.Add(filterByTypeBuilder.Build());

                if (!targetList.Any())
                    return;

                _walkthroughService.SetMainWalkthroughAsShown(typesToShow);

                var spotlight = new Spotlight(activity, targetList, 450);
                spotlight.Start();
            }
            catch (Exception)
            {
                
            }
        }

        
    }
}