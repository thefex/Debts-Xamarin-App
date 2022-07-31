using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Debts.Data;
using Debts.Droid.Activities;
using Debts.Resources;
using Humanizer;
using MvvmCross;
using MvvmCross.Platforms.Android;
using Newtonsoft.Json;

namespace Debts.Droid.Services.Notifications
{
    public class DroidNotificationCompatBuilder
    {
        private static int _id = 0;
        public const string CustomDataJsonKey = "PushCustomDataJson";
        private const string ChannelId = "default_debts_channel";

        public void ShowNotification(Context context, FinanceOperation operation)
        {
            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            var notification = 
                ConfigureNotificationBuilder(context, operation, new NotificationCompat.Builder(context, ChannelId), notificationManager).Build();
            
            int notificationId = System.Threading.Interlocked.Increment(ref _id);
            notificationManager.Notify(notificationId, notification);
        }

        protected PendingIntent GetPendingIntent(Context context, FinanceOperation notificationData){
            Intent activityIntent = new Intent(context, GetPendingIntentActivityType());

            var serializedCustomData = JsonConvert.SerializeObject(notificationData, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            activityIntent.PutExtra(CustomDataJsonKey, serializedCustomData);
            
            bool isMainActivityPresented = (Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity) is MainActivity;
            
            PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, activityIntent, isMainActivityPresented ? PendingIntentFlags.UpdateCurrent : PendingIntentFlags.CancelCurrent);
            return pendingIntent;
        } 
        protected System.Type GetPendingIntentActivityType() => typeof(MainActivity);

        protected NotificationCompat.Builder ConfigureNotificationBuilder(Context context,
            FinanceOperation notificationData, NotificationCompat.Builder notificationBuilder, NotificationManager manager)
        {
            string title = GetNotificationTitle(notificationData);
            string content = GetNotificationContent(notificationData);
            var builder =
                notificationBuilder
                    .SetDefaults((int)(NotificationDefaults.All))
                    .SetSmallIcon(Resource.Mipmap.app_icon)
                    .SetAutoCancel(true)
                    .SetContentTitle(title)
                    .SetContentText(content)
                    .SetStyle(new NotificationCompat.BigTextStyle().BigText(content))
                    .SetContentIntent(GetPendingIntent(context, notificationData))
                    .SetVisibility(NotificationCompat.VisibilityPublic)
                    .SetPriority((int)NotificationPriority.Max);

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {

                NotificationChannel channel = new NotificationChannel(ChannelId,
                    "Debts",
                    Android.App.NotificationImportance.Max);
                channel.SetShowBadge(true);
                manager.CreateNotificationChannel(channel);
                builder.SetChannelId(ChannelId);
            }

            return builder;
        }

        string GetNotificationTitle(FinanceOperation forOperation)
        {
            if (forOperation.IsDebt)
            {
                if (DateTime.UtcNow < forOperation.PaymentDetails.DeadlineDate)
                    return TextResources.Notifications_UpcomingDebt;

                return TextResources.Notifications_UnpaidDebt;
            }

            if (DateTime.UtcNow < forOperation.PaymentDetails.DeadlineDate)
                return TextResources.Notifications_UpcomingLoan;
            
            return TextResources.Notifications_UnpaidLoan;
        }

        string GetNotificationContent(FinanceOperation forOperation)
        {
            var humanizedDeadline = new DateTimeOffset(forOperation.PaymentDetails.DeadlineDate).Humanize(culture: new CultureInfo("en-us"));

            const string amountKey = "$AMOUNT$";
            const string currencyKey = "$CURRENCY$";
            const string forNameKey = "$CONTACT_NAME$";
            const string dateKey = "$DATE_OPERATION$";

            if (forOperation.IsDebt)
            {
                if (DateTime.UtcNow < forOperation.PaymentDetails.DeadlineDate)
                {

                    return
                        TextResources.Notifications_Template_UpcomingDebt
                            .Replace(amountKey, forOperation.PaymentDetails.Amount.ToString())
                            .Replace(currencyKey, forOperation.PaymentDetails.Currency)
                            .Replace(forNameKey, forOperation.RelatedTo.ToString())
                            .Replace(dateKey, humanizedDeadline);
                }


                return TextResources.Notifications_Template_UnpaidDebt
                    .Replace(amountKey, forOperation.PaymentDetails.Amount.ToString())
                    .Replace(currencyKey, forOperation.PaymentDetails.Currency)
                    .Replace(forNameKey, forOperation.RelatedTo.ToString())
                    .Replace(dateKey, humanizedDeadline);
            }

            if (DateTime.UtcNow < forOperation.PaymentDetails.DeadlineDate)
            { 
                return TextResources.Notifications_Template_UpcomingLoan
                    .Replace(amountKey, forOperation.PaymentDetails.Amount.ToString())
                    .Replace(currencyKey, forOperation.PaymentDetails.Currency)
                    .Replace(forNameKey, forOperation.RelatedTo.ToString())
                    .Replace(dateKey, humanizedDeadline);
            }

            return TextResources.Notifications_Template_UnpaidLoan
                .Replace(amountKey, forOperation.PaymentDetails.Amount.ToString())
                .Replace(currencyKey, forOperation.PaymentDetails.Currency)
                .Replace(forNameKey, forOperation.RelatedTo.ToString())
                .Replace(dateKey, humanizedDeadline);
        }
    }
}