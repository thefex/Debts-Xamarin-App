using System;
using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages
{
    public class QuestionMessageDialogMvxMessage : MvxMessage
    {
        public string Title { get; }
        public string Content { get; } 

        public QuestionMessageDialogMvxMessage(string title, string content, object sender) : base(sender)
        {
            Title = title;
            Content = content;
        }

        public Action OnYes { get; set; }

        public Action OnNo { get; set; }

        public string CancelButtonText { get; set; } = "no, thanks";

        public string OkButtonText { get; set; } = "ok";

    }
}