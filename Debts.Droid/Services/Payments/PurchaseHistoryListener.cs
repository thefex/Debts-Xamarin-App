using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.BillingClient.Api;
using Android.Runtime;

namespace Debts.Droid.Services.Payments
{
    class PurchaseHistoryListener : Java.Lang.Object, IPurchaseHistoryResponseListener
    {
        readonly TaskCompletionSource<IList<PurchaseHistoryRecord>> _taskCompletionSource;


        public PurchaseHistoryListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public PurchaseHistoryListener(TaskCompletionSource<IList<PurchaseHistoryRecord>> taskCompletionSource)
        {
            _taskCompletionSource = taskCompletionSource;
        }

        public void OnPurchaseHistoryResponse(BillingResult p0, IList<PurchaseHistoryRecord> p1)
        {
            if (!_taskCompletionSource.Task.IsCompleted)
                _taskCompletionSource.SetResult(p1);
        }
    }
}