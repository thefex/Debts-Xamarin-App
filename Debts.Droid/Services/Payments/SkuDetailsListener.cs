using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.BillingClient.Api;
using Android.Runtime;

namespace Debts.Droid.Services.AppGrowth
{
    class SkuDetailsListener : Java.Lang.Object, ISkuDetailsResponseListener
    {
        private readonly TaskCompletionSource<List<SkuDetails>> _skuDetailsTaskCompletionSource;

        public SkuDetailsListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public SkuDetailsListener(TaskCompletionSource<List<SkuDetails>> skuDetailsTaskCompletionSource)
        {
            _skuDetailsTaskCompletionSource = skuDetailsTaskCompletionSource;
        }

        public void OnSkuDetailsResponse(BillingResult p0, IList<SkuDetails> p1)
        {
            if (!_skuDetailsTaskCompletionSource.Task.IsCompleted)
            {
                if (p0.ResponseCode == BillingResponseCode.Ok && p1 != null)
                    _skuDetailsTaskCompletionSource.SetResult(p1.ToList());
                else
                    _skuDetailsTaskCompletionSource.SetException(
                        new InvalidOperationException(p0.ResponseCode.ToString() + p0.DebugMessage));
            }
        }
    }
}