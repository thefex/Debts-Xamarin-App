using System;
using System.Collections.Generic;
using Debts.Droid.Services.Walkthrough;
using Debts.iOS.ViewControllers.Base;
using Debts.Resources;
using Debts.Services.Settings;
using SpotlightXamarin;
using SpotlightXamarin.IOS;
using UIKit;

namespace Debts.iOS.Services.Walkthrough
{
    public class ContactListWalkthroughService : PlatformWalkthroughService
    {
        private readonly WalkthroughService _walkthroughService;

        public ContactListWalkthroughService(WalkthroughService walkthroughService)
        {
            _walkthroughService = walkthroughService;
        }
        
        public void ShowIfPossible()
        {
            try
            {
                if (_walkthroughService.IsContactListTutorialShown())
                    return;

                var mainViewController = GetMainViewController();
                
                UIView addView = mainViewController.BottomAppBarView.FloatingButton;
                UIView importView = mainViewController.BottomAppBarView.Subviews[2].Subviews[2].Subviews[0];
                
                var addTarget = new SimpleTargetBuilder(mainViewController)
                    .SetTitle(TextResources.WalkthroughService_ContactList_AddContact_Title)
                    .SetDescription(TextResources.WalkthroughService_ContactList_AddContact_Content)
                    .SetPoint(GetRelativeCenterPosition(addView, mainViewController))
                    .SetRadius((int)addView.Frame.Width)
                    .Build();

                var importSpotlight = new SimpleTargetBuilder(mainViewController)
                    .SetTitle(TextResources.WalkthroughService_ContactList_ImportContact_Title)
                    .SetDescription(TextResources.WalkthroughService_ContactList_ImportContact_Content)
                    .SetPoint(GetRelativeCenterPosition(importView, mainViewController))
                    .SetRadius((int)importView.Frame.Width)
                    .Build();


                _walkthroughService.SetContactListTutorialAsShown();
                var spotlight = new Spotlight(mainViewController, new List<Target>() {addTarget, importSpotlight}, 450);
                spotlight.Start();
            }
            catch (Exception)
            {
                
            }
        }
    }
}