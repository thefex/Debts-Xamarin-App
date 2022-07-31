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
    public class FinanceDetailsWalkthroughService
    {
        private readonly WalkthroughService _walkthroughService;
        private View finalizeView;
        private View shareView;
        private View smsView;
        private View callView;
        private View addNoteView;
        private View favoriteView;

        public FinanceDetailsWalkthroughService(WalkthroughService walkthroughService)
        {
            _walkthroughService = walkthroughService;
        }

        public void Initialize(View fabView, Activity activity)
        {
            finalizeView = fabView;
            shareView = activity.FindViewById(Resource.Id.action_finance_details_share);
            smsView = activity.FindViewById(Resource.Id.action_finance_details_sms);
            callView = activity.FindViewById(Resource.Id.action_finance_details_call);
            addNoteView = activity.FindViewById(Resource.Id.action_finance_details_add_note);
            favoriteView = activity.FindViewById(Resource.Id.action_finance_details_fav);
        }
        
        public void ShowIfPossible(Activity activity, bool isPhoneNumberAvailable, bool isCompleteAvailable)
        {
            try
            {
                var typesToShow = _walkthroughService.GetWalkthroughTypesToShowForFinanceDetailsView(isCompleteAvailable, isPhoneNumberAvailable);
                
                List<Target> targetList = new List<Target>();

                if (typesToShow.Contains(FinanceWalkthroughType.Finalize))
                {    
                    var finalizeTarget = new SimpleTargetBuilder(activity)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_FinalizeOperation_Title)
                        .SetDescription(TextResources.WalkthroughService_FinanceDetails_FinalizeOperation_Content)
                        .SetPoint(finalizeView)
                        .SetRadius(finalizeView.Width);

                    targetList.Add(finalizeTarget.Build());
                }

                if (typesToShow.Contains(FinanceWalkthroughType.AddNote))
                {
                    var addNoteBuilder = new SimpleTargetBuilder(activity)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_AddNote_Title)
                        .SetDescription(TextResources.WalkthrougService_FinanceDetails_AddNote_Content)
                        .SetPoint(addNoteView)
                        .SetRadius(addNoteView.Width);
                    
                    targetList.Add(addNoteBuilder.Build());
                }

                if (typesToShow.Contains(FinanceWalkthroughType.Call))
                {
                    var callTargetBuilder = new SimpleTargetBuilder(activity)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_PhoneCall_Title)
                        .SetDescription(TextResources.WalkthroughService_FinanceDetails_PhoneCall_Content)
                        .SetPoint(callView)
                        .SetRadius(callView.Width);
                    
                    targetList.Add(callTargetBuilder.Build());
                }

                if (typesToShow.Contains(FinanceWalkthroughType.Sms))
                {
                    var smsTargetBuilder = new SimpleTargetBuilder(activity)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_SMS_Title)
                        .SetDescription(TextResources.WalkthroughService_FinanceDetails_SMS_Content)
                        .SetPoint(smsView)
                        .SetRadius(smsView.Width);
                    
                    targetList.Add(smsTargetBuilder.Build());
                }

                if (typesToShow.Contains(FinanceWalkthroughType.Share))
                {
                    var shareTargetBuilder = new SimpleTargetBuilder(activity)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_Share_Title)
                        .SetDescription(TextResources.WalkthroughService_FinanceDetails_Share_Content)
                        .SetPoint(shareView)
                        .SetRadius(shareView.Width);
                    
                    targetList.Add(shareTargetBuilder.Build());
                }

                if (typesToShow.Contains(FinanceWalkthroughType.Favorite))
                {
                    var favoriteTargetBuilder = new SimpleTargetBuilder(activity)
                        .SetTitle(TextResources.WalkthroughService_FinanceDetails_FavoriteOperation_Title)
                        .SetDescription(TextResources.WalkthroughService_FinanceDetails_FavoriteOperation_Content)
                        .SetPoint(favoriteView)
                        .SetRadius(favoriteView.Width);
                    
                    targetList.Add(favoriteTargetBuilder.Build());
                }
                if (!targetList.Any())
                    return;

                _walkthroughService.SetFinanceDetailsWalkthroughAsShown(typesToShow);
                
                var spotlight = new Spotlight(activity, targetList, 450, new DecelerateInterpolator(2f));
                spotlight.Start();
            }
            catch (Exception)
            {
            }

            
        }
    }
}