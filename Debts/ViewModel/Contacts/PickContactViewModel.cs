using Debts.Commands;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.Messenging.Messages.App;
using Debts.Model;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Contacts;
using Debts.Services.Settings;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Debts.ViewModel.Contacts
{
    public class PickContactViewModel : ContactListViewModel
    {
        private readonly PremiumService _premiumService;

        public PickContactViewModel(QueryCommandExecutor queryCommandExecutor, 
            ImportContactsServices importContactsServices,
            WalkthroughService walkthroughService, 
            PremiumService premiumService,
            AdvertisementService advertisementService) : base(queryCommandExecutor, importContactsServices, walkthroughService, premiumService, advertisementService)
        {
            _premiumService = premiumService;
        }
        
        public MvxCommand<SelectableItem<ContactDetails>> ChildItemTapped => new MvxExceptionGuardedCommand<SelectableItem<ContactDetails>>((item) =>
        {
            ServicesLocation.NavigationService.Close(this);
            ServicesLocation.Messenger.Publish(new ContactSelectedMvxMessage(this, item.Item)); 
        });
        
        public MvxCommand Close => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.NavigationService.Close(this);
        });

        public bool AreAdsAvailable => _premiumService.AreAdsAvailable;
        
        public MvxCommand AddContact => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.NavigationService.Navigate<AddContactViewModel>();
        });
    }
}