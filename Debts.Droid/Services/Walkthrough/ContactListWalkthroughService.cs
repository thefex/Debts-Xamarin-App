using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Views.Animations;
using Debts.Droid.Activities;
using Debts.Resources;
using Debts.Services.Settings;
using SpotlightXamarin;

namespace Debts.Droid.Services.Walkthrough
{
    public class ContactListWalkthroughService
    {
        private readonly WalkthroughService _walkthroughService;

        public ContactListWalkthroughService(WalkthroughService walkthroughService)
        {
            _walkthroughService = walkthroughService;
        }
        
        public void ShowIfPossible(Activity activity, View importView)
        {
            try
            {
                if (_walkthroughService.IsContactListTutorialShown())
                    return;

                var addView = (activity as MainActivity).FloatingActionButton;

                var addTarget = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_ContactList_AddContact_Title)
                    .SetDescription(TextResources.WalkthroughService_ContactList_AddContact_Content)
                    .SetPoint(addView)
                    .SetRadius(addView.Width)
                    .Build();

                var importSpotlight = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_ContactList_ImportContact_Title)
                    .SetDescription(TextResources.WalkthroughService_ContactList_ImportContact_Content)
                    .SetPoint(importView)
                    .SetRadius(importView.Width)
                    .Build();


                _walkthroughService.SetContactListTutorialAsShown();
                var spotlight = new Spotlight(activity, new List<Target>() {addTarget, importSpotlight}, 450,
                    new DecelerateInterpolator(2f));
                spotlight.Start();
            }
            catch (Exception)
            {
                
            }
        }
    }
}