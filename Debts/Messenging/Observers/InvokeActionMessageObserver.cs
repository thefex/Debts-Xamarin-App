using System;
using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging
{
    public class InvokeActionMessageObserver<TMessage> : MvxMessageObserver<TMessage> where TMessage : MvxMessage
    {
        private readonly Action<TMessage> _actionToInvoke;

        public InvokeActionMessageObserver(Action<TMessage> actionToInvoke)
        {
            _actionToInvoke = actionToInvoke;
        }

        protected override void OnMessageArrived(TMessage messageToHandle)
        {
            _actionToInvoke(messageToHandle);
        }
    }
}