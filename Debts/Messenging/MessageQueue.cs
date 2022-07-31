using System;
using System.Collections.Generic;
using System.Linq;
using Debts.Services;
using MvvmCross;
using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging
{
    [Preserve]
    public class MessageQueue
    {
        private readonly IList<IDisposable> _messageSubscriptions = new List<IDisposable>();
        private readonly Dictionary<string, List<MvxMessage>> _messagesContainer = new Dictionary<string, List<MvxMessage>>();

        public void Init<ForTMessage>() where ForTMessage : MvxMessage
        {
   
            _messageSubscriptions.Add(ServicesLocation.Messenger.Subscribe<ForTMessage>(message =>
            {
                lock (this)
                {
                    if (ServicesLocation.Messenger.CountSubscriptionsFor<ForTMessage>() == 1)
                    {
                        var key = typeof(ForTMessage).ToString();
                        if (!_messagesContainer.ContainsKey(key))
                            _messagesContainer.Add(key, new List<MvxMessage>());

                        _messagesContainer[key].Add(message);
                    }
                }
            }));
        }

        public void ResendMessages<TForType>() where TForType : MvxMessage
        {
            lock (this)
            {
                string typeKey = typeof(TForType).ToString();

                if (!_messagesContainer.ContainsKey(typeKey))
                    return;

                foreach (var msg in _messagesContainer[typeKey].ToList())
                    ServicesLocation.Messenger.Publish(msg);
            }
        }

        public void ClearMessages<TForType>() where TForType : MvxMessage
        {
            lock (this)
            {
                string typeKey = typeof(TForType).ToString();

                if (_messagesContainer.ContainsKey(typeKey))
                    _messagesContainer[typeKey].Clear();
            }
        }

        public void ResendAndClearMessages<TForType>() where TForType : MvxMessage
        {
            lock (this)
            {
                ResendMessages<TForType>();
                ClearMessages<TForType>();
            }
        }
    }
}