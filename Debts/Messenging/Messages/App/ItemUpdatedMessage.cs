using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages.App
{
    public class ItemUpdatedMessage<TItem> : MvxMessage
    {
        public TItem UpdatedItem { get; }

        public ItemUpdatedMessage(object sender, TItem updatedItem) : base(sender)
        {
            UpdatedItem = updatedItem;
        }
    }
}