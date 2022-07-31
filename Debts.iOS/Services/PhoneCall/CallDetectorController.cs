using CallKit;
using CoreFoundation;
using Debts.Services.Phone;
using Foundation;
using MvvmCross;

namespace Debts.iOS.Services
{
    public class CallDetectorController : NSObject, ICXCallObserverDelegate
    {
        CXCallObserver observer;
        public void Initialize()
        {
            observer = new CXCallObserver();
            observer.SetDelegate(this, DispatchQueue.MainQueue);
        }

        public void CallChanged(CXCallObserver callObserver, CXCall call)
        {
            var callService = Mvx.IoCProvider.Resolve<PhoneCallServices>();
            if (!call.HasEnded && call.HasConnected)
                callService.OnConnected();
            else if (call.HasEnded && call.HasConnected)
                callService.OnDisconnected();
        }
    }
}