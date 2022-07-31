using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Views.Animations;
using Debts.Resources;
using Debts.Services.Settings;
using SpotlightXamarin;

namespace Debts.Droid.Services.Walkthrough
{
    public class StatisticsWalkthroughService
    {
        private readonly WalkthroughService _walkthroughService;

        public StatisticsWalkthroughService(WalkthroughService walkthroughService)
        {
            _walkthroughService = walkthroughService;
        }

        public void ShowIfPossible(Activity activity, View dateFilterMenuItem)
        {
            try
            {
                var dateSpotlight = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Statistics_FilterByDate_Title)
                    .SetDescription(TextResources.WalkthroughService_Statistics_FilterByDate_Content)
                    .SetPoint(dateFilterMenuItem)
                    .SetRadius(dateFilterMenuItem.Width)
                    .Build();

                if (_walkthroughService.IsStatisticsTutorialShown())
                    return;

                _walkthroughService.SetStatisticsTutorialAsShown();
                var spotlight = new Spotlight(activity, new List<Target>() {dateSpotlight}, 450,
                    new DecelerateInterpolator(2f));
                spotlight.Start();
            }
            catch (Exception)
            {
                
            }
        }
    }
}