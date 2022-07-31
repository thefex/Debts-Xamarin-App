using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Debts.Messenging.Messages;
using Debts.Services;

namespace Debts.Commands
{
    public abstract class AsyncGuardedCommandBuilder<TItem>
    {
        private readonly Queue<Action> _invokeAfterCommandExecutedActionQueue = new Queue<Action>();

        public MvxExceptionGuardedCommand<TItem> BuildCommand()
        {
            return new MvxExceptionGuardedCommand<TItem>(async (model) =>
            {
                try
                {
                    if (ShouldNotifyAboutProgress)
                        ServicesLocation.Messenger.Publish(LongRunningAsyncOperationMvxMessage.BuildOperationStartedMessage(this));
                    await ExecuteCommandAction(model).ConfigureAwait(false);
                }
                finally
                {
                    if (ShouldNotifyAboutProgress)
                        ServicesLocation.Messenger.Publish(LongRunningAsyncOperationMvxMessage.BuildOperationFinishedMessage(this));

                    while (_invokeAfterCommandExecutedActionQueue.Any())
                        _invokeAfterCommandExecutedActionQueue.Dequeue().Invoke();
                }
            });
        }
        protected virtual bool ShouldNotifyAboutProgress { get; } = true;
        protected abstract Task ExecuteCommandAction(TItem item);

        protected void EnqueueAfterCommandExecuted(Action actionToInvoke)
            => _invokeAfterCommandExecutedActionQueue.Enqueue(actionToInvoke);

    }

    public abstract class AsyncGuardedCommandBuilder
    {
        private readonly Queue<Action> _invokeAfterCommandExecutedActionQueue = new Queue<Action>();

        public MvxExceptionGuardedCommand BuildCommand()
        {
            return new MvxExceptionGuardedCommand(async () =>
            {
                try
                {
                    if (ShouldNotifyAboutProgress)
                        ServicesLocation.Messenger.Publish(LongRunningAsyncOperationMvxMessage.BuildOperationStartedMessage(this));
                    await ExecuteCommandAction().ConfigureAwait(false);
                }
                finally
                {
                    if (ShouldNotifyAboutProgress)
                        ServicesLocation.Messenger.Publish(LongRunningAsyncOperationMvxMessage.BuildOperationFinishedMessage(this));

                    while (_invokeAfterCommandExecutedActionQueue.Any())
                        _invokeAfterCommandExecutedActionQueue.Dequeue().Invoke();
                }
            });
        }

        protected virtual bool ShouldNotifyAboutProgress { get; } = true;

        protected abstract Task ExecuteCommandAction();

        protected void EnqueueAfterCommandExecuted(Action actionToInvoke)
            => _invokeAfterCommandExecutedActionQueue.Enqueue(actionToInvoke);

    }
}