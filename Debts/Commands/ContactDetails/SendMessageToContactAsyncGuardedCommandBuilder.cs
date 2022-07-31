using System.Threading.Tasks;
using Debts.Messenging.Messages.App;
using Debts.Services;
using Xamarin.Essentials;

namespace Debts.Commands.ContactDetails
{
    public class SendMessageToContactAsyncGuardedCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly Data.ContactDetails _contactDetails;

        public SendMessageToContactAsyncGuardedCommandBuilder(Data.ContactDetails contactDetails)
        {
            _contactDetails = contactDetails;
        }
        protected override async Task ExecuteCommandAction()
        {
            await Sms.ComposeAsync(new SmsMessage(string.Empty, _contactDetails.PhoneNumber));
            ServicesLocation.Messenger.Publish(new SharedOrSmsNoteMessage(this));
        }
    }
}