using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Views.Animations;
using Debts.Resources;
using Debts.Services.Settings;
using MvvmCross.Binding.BindingContext;
using SpotlightXamarin;

namespace Debts.Droid.Services.Walkthrough
{
    public class BudgetWalkthroughService
    {
        private readonly WalkthroughService _walkthroughService;
        private View addView;
        private View dateView;
        private View filterView;

        public BudgetWalkthroughService(WalkthroughService walkthroughService)
        {
            _walkthroughService = walkthroughService;
        }

        public void Initialize(View addView, Activity activity)
        {
            this.addView = addView;
            dateView = activity.FindViewById(Resource.Id.budget_list_nav_calendar);
            filterView = activity.FindViewById(Resource.Id.budget_list_nav_filter);
        }
        
        public void ShowIfPossible(Activity activity)
        {
            try
            {
                if (_walkthroughService.IsBudgetListTutorialShown())
                    return;
                
                _walkthroughService.SetBudgetListTutorialAsShown();

                var addTargetBuilder = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Budget_AddOperation_Title)
                    .SetDescription(TextResources.WalkthroughService_Budget_AddOperation_Content)
                    .SetPoint(addView)
                    .SetRadius(addView.Width);

                var dateTargetBuilder = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Budget_FilterByDate_Title)
                    .SetDescription(TextResources.WalkthroughService_Budget_FilterByDate_Content)
                    .SetPoint(dateView)
                    .SetRadius(dateView.Width);
                
                var filterTargetBuilder = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_Budget_FilterByCategory_Title)
                    .SetDescription(TextResources.WalkthroughService_Budget_FilterByCategory_Content)
                    .SetPoint(filterView)
                    .SetRadius(filterView.Width);

                List<Target> targetList = new List<Target>()
                {
                    addTargetBuilder.Build(),
                    dateTargetBuilder.Build(),
                    filterTargetBuilder.Build()
                };
 
                var spotlight = new Spotlight(activity, targetList, 450, new DecelerateInterpolator(2f));
                spotlight.Start();
            }
            catch (Exception)
            {
                
            }
        }
    }
}