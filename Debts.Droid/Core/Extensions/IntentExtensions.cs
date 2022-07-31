using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.Gms.Tasks;
using Android.Runtime;
using Debts.Data;
using Debts.Droid.Activities;
using Debts.Droid.Services.Notifications;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Notifications;
using Firebase.DynamicLinks;
using Java.Lang;
using MvvmCross;
using Newtonsoft.Json;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;
using Task = System.Threading.Tasks.Task;

namespace Debts.Droid.Core.Extensions
{
    class DeepLinkSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        public DeepLinkSuccessListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public DeepLinkSuccessListener()
        {
        }

        public void OnSuccess(Object result)
        {
            if (result == null)
                return;

            try
            {
                PendingDynamicLinkData castedResult = result.JavaCast<PendingDynamicLinkData>();
                var tappedLink = castedResult.Link.ToString();

                var replacedLink = tappedLink
                    .Replace("https://www.go-debts.com/#?operation=", "")
                    .Replace("%3D", "=");

                var byteJson =
                    System.Convert.FromBase64String(replacedLink);
                var jsonString = System.Text.Encoding.UTF8.GetString(byteJson);

                var financeOperation = JsonConvert.DeserializeObject<FinanceOperation>(jsonString);

                Mvx.IoCProvider.Resolve<DeepLinkHandler>().HandleDeepLink(financeOperation);
            }
            catch (System.Exception e)
            {
                ServicesLocation.ExceptionGuard.OnException(e);
            }
        }
    }

    class DeepLinkFailureListener : Java.Lang.Object, IOnFailureListener
    {
        public DeepLinkFailureListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public DeepLinkFailureListener()
        {
        }

        public void OnFailure(Exception e)
        {
            if (e == null)
                return;
            ServicesLocation.ExceptionGuard.OnException(e);
        }
    }
    
    public static class IntentExtensions
    {
        public static void TryHandleMainActivityIntentData(this Intent intent, MainActivity mainActivity)
        {
            if (intent == null)
                return;
             
            if (intent?.Extras?.ContainsKey(DroidNotificationCompatBuilder.CustomDataJsonKey) ?? false)
            {
                string customJson = intent.Extras.GetString(DroidNotificationCompatBuilder.CustomDataJsonKey);
                intent.RemoveExtra(DroidNotificationCompatBuilder.CustomDataJsonKey);

                var financeOperation = JsonConvert.DeserializeObject<FinanceOperation>(customJson);
                Mvx.IoCProvider.Resolve<FinanceOperationNotificationHandler>().HandleNotification(financeOperation);
                return;
            }
            /* UNCOMMENT THIS WHEN YOU PROVIDE CONSTANTS AS DESCRIBED IN README FILE!
            FirebaseDynamicLinks.Instance
                .GetDynamicLink(intent)
                .AddOnSuccessListener(mainActivity, new DeepLinkSuccessListener())
                .AddOnFailureListener(mainActivity, new DeepLinkFailureListener()); 
            */
        }
    }
}