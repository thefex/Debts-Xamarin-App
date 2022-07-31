using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging
{
    public class MvxMessageObserversController : IDisposable
    {
        private readonly IList<IMvxMessageObserver> _messageObservers;
        private readonly IMvxMessenger _messenger;

        public MvxMessageObserversController(IMvxMessenger messenger)
        {
            _messenger = messenger;
            _messageObservers = new List<IMvxMessageObserver>();
        }

        public int Count => _messageObservers.Count;

        public virtual void Dispose()
        {
            StopObserving();
        }

        public MvxMessageObserversController AddObservers(IMvxMessageObserver messageObserver)
        {
            _messageObservers.Add(messageObserver);
            return this;
        }

        public MvxMessageObserversController AddObservers(IEnumerable<IMvxMessageObserver> messageObservers)
        {
            foreach (var observer in messageObservers)
                AddObservers(observer);
            return this;
        }

        public MvxMessageObserversController RemoveObserver(IMvxMessageObserver messageObserver)
        {
            _messageObservers.Remove(messageObserver);
            return this;
        }

        public virtual void StartObserving()
        {
            foreach (var observer in _messageObservers.Where(x => !x.IsObserving).ToList())
                observer.Start(_messenger);
        }

        public virtual void StopObserving()
        {
            foreach (var observer in _messageObservers.Where(x => x.IsObserving).ToList())
                observer.Stop();
        }
    }
}