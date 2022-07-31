using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages
{
    public class MessageDialogMvxMessage : MvxMessage
    {
        public string Title { get; }
        public string Content { get; }
        
         
        public MessageDialogMvxMessage(string title, string content, object sender) : base(sender)
        {
            Title = title;
            Content = content;
        }

    }
}