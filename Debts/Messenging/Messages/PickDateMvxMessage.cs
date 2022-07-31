using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages
{
    public class PickDateMvxMessage : MvxMessage
    {
        public PickDateMvxMessage(object sender) : base(sender)
        {
        }

        public string Tag { get; set; } = string.Empty;
    }
}