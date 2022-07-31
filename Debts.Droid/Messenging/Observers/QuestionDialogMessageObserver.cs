using System;
using Android.Support.V7.App;
using Debts.Droid.Core.Extensions;
using Debts.Messenging;
using Debts.Messenging.Messages;

namespace Debts.Droid.Messenging.Observers
{
    internal class QuestionDialogMessageObserver : MvxMessageObserver<QuestionMessageDialogMvxMessage>
    {
        private readonly AppCompatActivity _activityReference;

        public QuestionDialogMessageObserver(AppCompatActivity activityReference)
        {
            _activityReference = activityReference;
        }

        protected override void OnMessageArrived(QuestionMessageDialogMvxMessage messageToHandle)
        {
            _activityReference.ShowQuestionDialog(messageToHandle.Title, messageToHandle.Content, messageToHandle.OnYes, messageToHandle.OnNo,
                messageToHandle.CancelButtonText,
                messageToHandle.OkButtonText);
        }
    }
}