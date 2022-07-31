using Android.Support.V7.App;
using Debts.Droid.Core.Extensions;
using Debts.Messenging;
using Debts.Messenging.Messages;

namespace Debts.Droid.Messenging.Observers
{
    internal class MessageDialogMessageObserver : MvxMessageObserver<MessageDialogMvxMessage>
    {
        private readonly AppCompatActivity _activityReference;

        public MessageDialogMessageObserver(AppCompatActivity activityReference)
        {
            _activityReference = activityReference;
        }

        protected override void OnMessageArrived(MessageDialogMvxMessage messageToHandle)
        {
            _activityReference.ShowMessageDialog(messageToHandle.Title, messageToHandle.Content);
        }
    }
}