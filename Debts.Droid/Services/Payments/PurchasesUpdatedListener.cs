using System;
using System.Collections.Generic;
using Android.BillingClient.Api;
using Android.Runtime;

namespace Debts.Droid.Services.AppGrowth
{
    class PurchasesUpdatedListener : Java.Lang.Object, IPurchasesUpdatedListener
    {
        public PurchasesUpdatedListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public PurchasesUpdatedListener()
        {
        }

        public void OnPurchasesUpdated(BillingResult p0, IList<Purchase> p1)
        {
            if (p0.ResponseCode == BillingResponseCode.Ok)
                Purchased?.Invoke(p1);
        }

        public event Action<IList<Purchase>> Purchased;
    }
}