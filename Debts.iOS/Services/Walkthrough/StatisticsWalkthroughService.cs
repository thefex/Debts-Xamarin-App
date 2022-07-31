using System;
using System.Collections.Generic;
using Debts.Resources;
using Debts.Services.Settings;
using SpotlightXamarin.IOS;
using UIKit;

namespace Debts.Droid.Services.Walkthrough
{
    public class StatisticsWalkthroughService
    {
        private readonly WalkthroughService _walkthroughService;

        public StatisticsWalkthroughService(WalkthroughService walkthroughService)
        {
            _walkthroughService = walkthroughService;
        }

        public void ShowIfPossible(UIViewController activity, UIView dateFilterMenuItem)
        {
            try
            {
                var dateSpotlight = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Statistics_FilterByDate_Title)
                    .SetDescription(TextResources.WalkthroughService_Statistics_FilterByDate_Content)
                    .SetPoint(dateFilterMenuItem)
                    .SetRadius((int)dateFilterMenuItem.Frame.Width)
                    .Build();

                if (_walkthroughService.IsStatisticsTutorialShown())
                    return;

                _walkthroughService.SetStatisticsTutorialAsShown();
                var spotlight = new Spotlight(activity, new List<Target>() {dateSpotlight}, 450);
                    spotlight.Start();
            }
            catch (Exception)
            {
                
            }
        }
    }
}