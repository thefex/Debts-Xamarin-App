using MvvmCross.Plugin.Messenger;

namespace Debts.Messenging
{
    public interface IMvxMessageObserver
    {
        bool IsObserving { get; }
        void Start(IMvxMessenger messenger);
        void Stop();
    }
}