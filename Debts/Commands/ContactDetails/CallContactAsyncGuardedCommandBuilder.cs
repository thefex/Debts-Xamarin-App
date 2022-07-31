using System.Threading.Tasks;
using Debts.Messenging.Messages;
using Debts.Resources;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Phone;
using Debts.ViewModel.AppGrowth;
using MvvmCross;
using Xamarin.Essentials;

namespace Debts.Commands.ContactDetails
{
    public class CallContactAsyncGuardedCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly PhoneCallServices _phoneCallServices;
        private readonly Data.ContactDetails _contactDetails;
        private readonly PremiumService _premiumService;

        public CallContactAsyncGuardedCommandBuilder(PhoneCallServices phoneCallServices, Data.ContactDetails contactDetails, PremiumService premiumService)
        {
            _phoneCallServices = phoneCallServices;
            _contactDetails = contactDetails;
            _premiumService = premiumService;
        }

        protected override bool ShouldNotifyAboutProgress => false;

        protected override async Task ExecuteCommandAction()
        {
            if (!_premiumService.HasPremium)
            {
                await ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
                return;
            }
            
            var phoneCallStatePermission = Mvx.IoCProvider.Resolve<IPhoneCallPermission>();
            if (!await phoneCallStatePermission.RequestPhoneCallStatePermission())
            {
                ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, TextResources.Command_CallContact_PermissionRequired)
                {
                    Style = ToastMvxMessage.ToastStyle.Error
                });
                return;
            }
            _phoneCallServices.StartPhoneCall(_contactDetails.PhoneNumber);        
        }
    }
}