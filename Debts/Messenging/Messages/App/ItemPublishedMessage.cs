using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages.App
{
    public class ItemPublishedMessage<TItem> : MvxMessage
    {
        public TItem Item { get; }

        public ItemPublishedMessage(object sender, TItem item) : base(sender)
        {
            Item = item;
        }
    }
}