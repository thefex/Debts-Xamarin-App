using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages.App
{
    public class ItemRemovedMessage<TItem> : MvxMessage
    {
        public TItem Item { get; }

        public ItemRemovedMessage(object sender, TItem item) : base(sender)
        {
            Item = item;
        }
    }
}