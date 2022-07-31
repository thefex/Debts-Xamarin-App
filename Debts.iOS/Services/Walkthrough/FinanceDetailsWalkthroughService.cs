using System;
using System.Collections.Generic;
using System.Linq;
using Debts.Model.Walkthrough;
using Debts.Resources;
using Debts.Services.Settings;
using SpotlightXamarin.IOS;
using UIKit;

namespace Debts.Droid.Services.Walkthrough
{
    public class FinanceDetailsWalkthroughService : PlatformWalkthroughService
    {
        private readonly WalkthroughService _walkthroughService;

        public FinanceDetailsWalkthroughService(WalkthroughService walkthroughService)
        {
            _walkthroughService = walkthroughService;
        }

        public void Initialize(bool isCompleted, bool hasPhoneNumber)
        {
            var mainViewController = GetMainViewController();
            
            UIView phoneCall = mainViewController.BottomAppBarView.Subviews[2].Subviews[1].Subviews[2];
            UIView sms = mainViewController.BottomAppBarView.Subviews[2].Subviews[1].Subviews[3];
            mainViewController.BottomAppBarView.SetFloatingButtonHidden(isCompleted, true);

            phoneCall.Hidden = !hasPhoneNumber;
            sms.Hidden = !hasPhoneNumber;
            phoneCall.UserInteractionEnabled = hasPhoneNumber;
            sms.UserInteractionEnabled = hasPhoneNumber;
        }
        
        public void ShowIfPossible(UIViewController viewController, UIView favoriteView, UIView deleteView, bool isPhoneNumberAvailable, bool isCompleteAvailable)
        {
            try
            {
                var typesToShow = _walkthroughService.GetWalkthroughTypesToShowForFinanceDetailsView(isCompleteAvailable, isPhoneNumberAvailable);
             
                var mainViewController = GetMainViewController();
                List<Target> targetList = new List<Target>();

                if (typesToShow.Contains(FinanceWalkthroughType.Finalize))
                {    
                    UIView finalizeView = mainViewController.BottomAppBarView.FloatingButton;

                    var finalizeTarget = new SimpleTargetBuilder(mainViewController)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_FinalizeOperation_Title)
                        .SetDescription(TextResources.WalkthroughService_FinanceDetails_FinalizeOperation_Content)
                        .SetPoint(GetRelativeCenterPosition(finalizeView, viewController))
                        .SetRadius((int)finalizeView.Frame.Width);

                    targetList.Add(finalizeTarget.Build());
                }

                if (typesToShow.Contains(FinanceWalkthroughType.AddNote))
                {
                    UIView addNoteView = mainViewController.BottomAppBarView.Subviews[2].Subviews[1].Subviews[0];

                    var addNoteBuilder = new SimpleTargetBuilder(mainViewController)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_AddNote_Title)
                        .SetDescription(TextResources.WalkthrougService_FinanceDetails_AddNote_Content)
                        .SetPoint(GetRelativeCenterPosition(addNoteView, viewController))
                        .SetRadius((int)addNoteView.Frame.Width);
                    
                    targetList.Add(addNoteBuilder.Build());
                }

                if (typesToShow.Contains(FinanceWalkthroughType.Share))
                { 
                    UIView shareView = mainViewController.BottomAppBarView.Subviews[2].Subviews[1].Subviews[1];

                    var shareTargetBuilder = new SimpleTargetBuilder(mainViewController)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_Share_Title)
                        .SetDescription(TextResources.WalkthroughService_FinanceDetails_Share_Content)
                        .SetPoint(GetRelativeCenterPosition(shareView, viewController))
                        .SetRadius((int)shareView.Frame.Width);
                    
                    targetList.Add(shareTargetBuilder.Build());
                }
                
                if (typesToShow.Contains(FinanceWalkthroughType.Call))
                {
                    UIView callView = mainViewController.BottomAppBarView.Subviews[2].Subviews[1].Subviews[2];

                    var callTargetBuilder = new SimpleTargetBuilder(mainViewController)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_PhoneCall_Title)
                        .SetDescription(TextResources.WalkthroughService_FinanceDetails_PhoneCall_Content)
                        .SetPoint(GetRelativeCenterPosition(callView, viewController))
                        .SetRadius((int)callView.Frame.Width);
                    
                    targetList.Add(callTargetBuilder.Build());
                }

                if (typesToShow.Contains(FinanceWalkthroughType.Sms))
                {
                    UIView smsView = mainViewController.BottomAppBarView.Subviews[2].Subviews[1].Subviews[3];

                    var smsTargetBuilder = new SimpleTargetBuilder(mainViewController)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_SMS_Title)
                        .SetDescription(TextResources.WalkthroughService_FinanceDetails_SMS_Content)
                        .SetPoint(GetRelativeCenterPosition(smsView, viewController))
                        .SetRadius((int)smsView.Frame.Width);
                    
                    targetList.Add(smsTargetBuilder.Build());
                } 

                if (typesToShow.Contains(FinanceWalkthroughType.Favorite))
                {
                    var favoriteTargetBuilder = new SimpleTargetBuilder(mainViewController)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_FavoriteOperation_Title)
                        .SetDescription(TextResources.WalkthroughService_FinanceDetails_FavoriteOperation_Content)
                        .SetPoint(GetRelativeCenterPosition(favoriteView, viewController))
                        .SetRadius((int)favoriteView.Frame.Width);
                    
                    targetList.Add(favoriteTargetBuilder.Build());
                }
                if (!targetList.Any())
                    return;

                _walkthroughService.SetFinanceDetailsWalkthroughAsShown(typesToShow);
                
                var spotlight = new Spotlight(mainViewController, targetList, 450);
                spotlight.Start();
            }
            catch (Exception)
            {
            }

            
        }
    }
}