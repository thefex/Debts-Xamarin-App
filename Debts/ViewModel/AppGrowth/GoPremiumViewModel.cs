using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Data;
using Debts.Messenging.Messages;
using Debts.Resources;
using Debts.Services;
using Debts.Services.AppGrowth;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Debts.ViewModel.AppGrowth
{
    public class GoPremiumViewModel : MvxViewModel
    {
        private readonly AdvertisementService _advertisementService;
        private readonly PremiumService _premiumService;

        public GoPremiumViewModel(AdvertisementService advertisementService, PremiumService premiumService)
        {
            _advertisementService = advertisementService;
            _premiumService = premiumService;
        }

        public override Task Initialize()
        {
            if (_advertisementService.IsRewardAdAvailable)
                _advertisementService.PreloadRewardedAd();
            return base.Initialize();
        }
        

        public MvxCommand Skip => new MvxExceptionGuardedCommand(async () =>
            {
                bool showRewardedAd = false;
                
                if (_advertisementService.IsRewardAdAvailable && _advertisementService.IsRewardAdLoaded)
                {    
                    Subject<bool> questionResultSubject = new Subject<bool>();
                    ServicesLocation.Messenger.Publish(new QuestionMessageDialogMvxMessage(
                        "Extend trial", 
                        "Would you like to watch advertisement and get free access to premium features for one day?", this)
                    {
                        OnNo = () =>
                        {
                            questionResultSubject.OnNext(false);
                        },
                        OnYes = () =>
                        {
                            questionResultSubject.OnNext(true);
                        }
                    });

                    showRewardedAd = await questionResultSubject.FirstAsync();
                }
                
                await ServicesLocation.NavigationService.Close(this);

                if (showRewardedAd)
                {
                    bool hasRewardSucceed = await _advertisementService.ShowFullScreenRewardAd();

                    if (hasRewardSucceed)
                        _premiumService.ExtendTrialByOneDay();
                }
            });

        public bool HasMonthlySubscription => _premiumService.PremiumState == PremiumState.PremiumSubscription;
        
        public MvxCommand BuyApp => new MvxExceptionGuardedCommand(async () =>
        {
            await ServicesLocation.NavigationService.Close(this);
            if (!_premiumService.CanMakePayments)
            {
                ServicesLocation.Messenger.Publish(new MessageDialogMvxMessage(TextResources.Dialog_Error_Title, 
                    TextResources.Premium_CantBuy,
                    this));   
                return;
            }
            
            _premiumService.BuyLifetimePremium();
        });
        
        public MvxCommand BuySubscription => new MvxExceptionGuardedCommand(async () =>
            {
                await ServicesLocation.NavigationService.Close(this);
                if (!_premiumService.CanMakePayments)
                {
                    ServicesLocation.Messenger.Publish(new MessageDialogMvxMessage(TextResources.Dialog_Error_Title, 
                        TextResources.Premium_CantBuy,
                        this));   
                    return;
                }
                _premiumService.BuyMonthlySubscription();
            });

        public string GoPremiumText => "Remove trial and gain following benefits:" + Environment.NewLine + Environment.NewLine +
                                       "• Completely free of advertisements!" + Environment.NewLine +
                                       "• Access to your favorite finances list" + Environment.NewLine +
                                       "• Filter finances list by status or date" + Environment.NewLine +
                                       "• Filter statistics by date" + Environment.NewLine +
                                       "• See on map where your finance operation was created" + Environment.NewLine +
                                       "• Call through application" + Environment.NewLine +
                                       "• Add finance operation note"
                                       + Environment.NewLine
                                       + Environment.NewLine +
                                       "...be like happy bear - go with happy premium!";
    }
}