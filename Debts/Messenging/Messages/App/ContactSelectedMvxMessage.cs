using Debts.Data;
using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages.App
{
    public class ContactSelectedMvxMessage : MvxMessage
    {
        public ContactSelectedMvxMessage(object sender, ContactDetails selectedContact) : base(sender)
        {
            SelectedContact = selectedContact;
        }

        public ContactDetails SelectedContact { get; }
    }
}