using System.Windows.Input;
using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages
{
    public class ToastMvxMessage : MvxMessage
    {
        public string Content { get; }

        public ToastStyle Style { get; set; } = ToastStyle.Info;

        public string ActionText { get; set; } = string.Empty;
        
        public ICommand ActionCommand { get; set; } = null;

        public ICommand DismissCommand { get; set; } = null;

        public ToastMvxMessage(object sender, string content) : base(sender)
        {
            Content = content;
        }

        public enum ToastStyle
        {
            Info,
            Error
        }
    }
}