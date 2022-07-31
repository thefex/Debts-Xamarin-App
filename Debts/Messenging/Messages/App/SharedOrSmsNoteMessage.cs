using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages.App
{
    public class SharedOrSmsNoteMessage : MvxMessage
    {
        public SharedOrSmsNoteMessage(object sender) : base(sender)
        {
        }
    }
}