using Android.Support.V7.App;
using Debts.Droid.Core.Extensions;
using Debts.Messenging;
using Debts.Messenging.Messages;

namespace Debts.Droid.Messenging.Observers
{
    internal class CheckBoxQuestionDialogMessageObserver : MvxMessageObserver<QuestionMessageWithCheckBoxMvxMessage>
    {
        private readonly AppCompatActivity _activityReference;

        public CheckBoxQuestionDialogMessageObserver(AppCompatActivity activityReference)
        {
            _activityReference = activityReference;
        }

        protected override void OnMessageArrived(QuestionMessageWithCheckBoxMvxMessage messageToHandle)
        {
            _activityReference.ShowQuestionDialog(messageToHandle.Title, messageToHandle.Content,  messageToHandle.CheckBoxText,
                messageToHandle.OnYes,
                messageToHandle.OnNo);
        }
    }
}