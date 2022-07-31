using System;
using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages
{
    public class QuestionMessageWithCheckBoxMvxMessage : MvxMessage
    {
        public string Title { get; }
        public string Content { get; } 

        public QuestionMessageWithCheckBoxMvxMessage(string title, string content, object sender) : base(sender)
        {
            Title = title;
            Content = content;
        }

        public string CheckBoxText { get; set; } = "Never ask again";
        
        public Action<bool> OnYes { get; set; }

        public Action<bool> OnNo { get; set; }
    }
}