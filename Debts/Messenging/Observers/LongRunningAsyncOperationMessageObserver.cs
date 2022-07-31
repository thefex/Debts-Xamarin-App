using System;
using System.Collections.Generic;
using System.Linq;
using Debts.Messenging.Messages;

namespace Debts.Messenging
{
    public abstract class LongRunningAsyncOperationMessageObserver : MvxMessageObserver<LongRunningAsyncOperationMvxMessage>
    {
        private int _currentAsyncOperationInProgress;

        /// <summary>
        ///     By default all sender types are proccessed.
        /// </summary>
        public IEnumerable<Type> SenderTypesThatShouldBeProccesed { get; set; } = Enumerable.Empty<Type>();

        protected sealed override void OnMessageArrived(LongRunningAsyncOperationMvxMessage messageToHandle)
        {
            _currentAsyncOperationInProgress += messageToHandle.HasStarted ? 1 : -1;

            if (_currentAsyncOperationInProgress == 1 && messageToHandle.HasStarted)
                OnAsyncOperationStarted();
            else if (_currentAsyncOperationInProgress == 0 && messageToHandle.HasFinished)
                OnAsyncOperationFinished();
        }

        protected virtual void OnAsyncOperationStarted()
        {
        }

        protected virtual void OnAsyncOperationFinished()
        {
        }

        protected override bool ShouldHandleMessage(LongRunningAsyncOperationMvxMessage message)
        {
            if (!SenderTypesThatShouldBeProccesed.Any())
                return base.ShouldHandleMessage(message);

            return SenderTypesThatShouldBeProccesed.Contains(message.Sender.GetType());
        }
    }
}