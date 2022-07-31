using System;
using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging.Messages
{
    public class LongRunningAsyncOperationMvxMessage : MvxMessage
    {
        public LongRunningAsyncOperationMvxMessage(object sender) : base(sender)
        {
        }

        public bool HasStarted { get; private set; }

        public bool HasFinished { get; private set; }

        public static LongRunningAsyncOperationMvxMessage BuildOperationStartedMessage(object sender)
        {
            return new LongRunningAsyncOperationMvxMessage(sender)
            {
                HasStarted = true
            };
        }

        public static LongRunningAsyncOperationMvxMessage BuildOperationFinishedMessage(object sender)
        {
            return new LongRunningAsyncOperationMvxMessage(sender)
            {
                HasFinished = true
            };
        }
    } 
}