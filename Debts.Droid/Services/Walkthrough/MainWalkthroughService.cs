using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.Animations;
using Debts.Model.Walkthrough;
using Debts.Resources;
using Debts.Services.Settings;
using SpotlightXamarin;

namespace Debts.Droid.Services.Walkthrough
{
    public class MainWalkthroughService
    {
        private readonly WalkthroughService _walkthroughService;
        private View addView;
        private View menuView;
        private View dateView;
        private View filterView;

        public MainWalkthroughService(WalkthroughService walkthroughService)
        {
            _walkthroughService = walkthroughService;
        }

        public void Initialize(View addView, Activity activity)
        {
            this.addView = addView;
            menuView = activity.FindViewById(Resource.Id.menu_tutorial_stub_view);
            dateView = activity.FindViewById(Resource.Id.finance_list_nav_calendar);
            filterView = activity.FindViewById(Resource.Id.finance_list_nav_filter);
        }
        
        public void ShowIfPossible(Activity activity)
        {
            try
            {
                var typesToShow = _walkthroughService.GetWalkthroughTypesToShowForMainView();

                var addTargetBuilder = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Main_AddOperation_Title)
                    .SetDescription(TextResources.WalkthroughService_Main_AddOperation_Content)
                    .SetPoint(addView)
                    .SetRadius(addView.Width);

                var menuTargetBuilder = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Main_Menu_Title)
                    .SetDescription(TextResources.WalkthroughService_Main_Menu_Content)
                    .SetPoint(menuView)
                    .SetRadius(menuView.Width);

                var dateTargetBuilder = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Main_FilterByDate_Title)
                    .SetDescription(TextResources.WalkthroughService_Main_FilterByDate_Content)
                    .SetPoint(dateView)
                    .SetRadius(dateView.Width);

                var filterByTypeBuilder = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Main_FilterByStatus_Title)
                    .SetDescription(TextResources.WalkthroughService_Main_FilterByStatus_Content)
                    .SetPoint(filterView)
                    .SetRadius(filterView.Width);

                List<Target> targetList = new List<Target>();

                if (typesToShow.Contains(MainWalkthroughType.Add))
                    targetList.Add(addTargetBuilder.Build());
                if (typesToShow.Contains(MainWalkthroughType.Menu))
                    targetList.Add(menuTargetBuilder.Build());
                if (typesToShow.Contains(MainWalkthroughType.FilterByDates))
                    targetList.Add(dateTargetBuilder.Build());
                if (typesToShow.Contains(MainWalkthroughType.FilterByType))
                    targetList.Add(filterByTypeBuilder.Build());

                if (!targetList.Any())
                    return;

                _walkthroughService.SetMainWalkthroughAsShown(typesToShow);

                var spotlight = new Spotlight(activity, targetList, 450, new DecelerateInterpolator(2f));
                spotlight.Start();
            }
            catch (Exception)
            {
                
            }
        }
    }
}