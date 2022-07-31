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
    public class ImportContactsWalkthroughService
    {
        private readonly WalkthroughService _walkthroughService;

        public ImportContactsWalkthroughService(WalkthroughService walkthroughService)
        {
            _walkthroughService = walkthroughService;
        }

        public void ShowIfPossible(Activity activity, View searchImportedContacts, View selectMenu)
        {
            try
            {
                if (_walkthroughService.IsImportContactsTutorialShown())
                    return;

                var searchSpotlight = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_ImportContacts_LookingForContact_Title)
                    .SetDescription(TextResources.WalkthroughService_ImportContacts_LookingForContact_Content)
                    .SetPoint(searchImportedContacts)
                    .SetRadius(searchImportedContacts.Width)
                    .Build();

                var selectAllSpotlight = new SimpleTargetBuilder(activity)
                    .SetTitle(TextResources.WalkthroughService_ImportContacts_Utilities_Title)
                    .SetDescription(TextResources.WalkthroughService_ImportContacts_Utilities_Content)
                    .SetPoint(selectMenu)
                    .SetRadius(selectMenu.Width)
                    .Build();


                _walkthroughService.SetImportContactsTutorialAsShown();
                var spotlight = new Spotlight(activity, new List<Target>() {searchSpotlight, selectAllSpotlight}, 450,
                    new DecelerateInterpolator(2f));
                spotlight.Start();
            }
            catch (Exception)
            {
                
            }
        }
    }
}