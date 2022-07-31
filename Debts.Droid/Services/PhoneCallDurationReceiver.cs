using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Telephony;
using Debts.Services.Phone;
using MvvmCross;

namespace Debts.Droid.Services
{
    [BroadcastReceiver]
    [IntentFilter(new [] { "android.intent.action.PHONE_STATE" })]
    public class PhoneCallDurationReceiver : BroadcastReceiver
    {
        public PhoneCallDurationReceiver(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public PhoneCallDurationReceiver()
        {
        }

        
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                String action = intent.Action;
                if (action.Equals("android.intent.action.PHONE_STATE", StringComparison.OrdinalIgnoreCase))
                {
                    if (intent.GetStringExtra(TelephonyManager.ExtraState).Equals(
                        TelephonyManager.ExtraStateOffhook))
                    {
                        Mvx.IoCProvider.Resolve<PhoneCallServices>().OnConnected();
                    }

                    if (intent.GetStringExtra(TelephonyManager.ExtraState).Equals(
                        TelephonyManager.ExtraStateIdle))
                    {
                        Mvx.IoCProvider.Resolve<PhoneCallServices>().OnDisconnected();
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }
    }
}