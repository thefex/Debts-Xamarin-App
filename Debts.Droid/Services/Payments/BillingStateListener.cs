using System;
using System.Threading.Tasks;
using Android.BillingClient.Api;
using Android.Runtime;

namespace Debts.Droid.Services.AppGrowth
{
    class BillingStateListener : Java.Lang.Object, IBillingClientStateListener
    {
        private readonly BillingClient _billingClient;
        private readonly TaskCompletionSource<bool> _completionSource;

        public BillingStateListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public BillingStateListener(BillingClient billingClient, TaskCompletionSource<bool> completionSource)
        {
            _billingClient = billingClient;
            _completionSource = completionSource;
        }
         
        public void OnBillingServiceDisconnected()
        {
            _billingClient.StartConnection(this);
        }

        public void OnBillingSetupFinished(BillingResult p0)
        {
            if (!_completionSource.Task.IsCompleted)
                _completionSource.SetResult(p0.ResponseCode == BillingResponseCode.Ok);
        }
    }
}