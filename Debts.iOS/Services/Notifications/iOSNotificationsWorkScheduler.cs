using System;
using System.Globalization;
using BackgroundTasks;
using CoreFoundation;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Resources;
using Debts.Services;
using Debts.Services.Notifications;
using Debts.Services.Settings;
using Foundation;
using Humanizer;
using MvvmCross;
using Newtonsoft.Json;
using UIKit;
using UserNotifications;

namespace Debts.iOS.Services.Notifications
{
    public class iOSNotificationsWorkScheduler : INotificationsWorkScheduler
    {
        public void StartRepeatedBackgroundJobs()
        {
           
        }

        public static void Register()
        {
            BGTaskScheduler.Shared.CancelAll();
            BGTaskScheduler.Shared.Register("com.debts.NotificationTask",
            null,
            HandleBackgroundJob);
        }

        /// <summary>
        ///  on iOS it has to be executed before app starts launching...
        /// </summary>
        public static void Start()
        {
            ScheduleNewBackgroundJob();
        }

        internal static void HandleBackgroundJob(BGTask task)
        {

            var appRefreshTask = task as BGAppRefreshTask;
            appRefreshTask.ExpirationHandler = () => { appRefreshTask.SetTaskCompleted(false); };
             
            ScheduleNewBackgroundJob();
            OnTaskRun();
            appRefreshTask.SetTaskCompleted(true);

            return;
        } 
        
        private static void ScheduleNewBackgroundJob()
        {
            var request = new BGAppRefreshTaskRequest("com.debts.NotificationTask");
            
            request.EarliestBeginDate = NSDate.Now.AddSeconds(60*15);

            try
            {
                NSError error = null;
                BGTaskScheduler.Shared.Submit(request, out error);
                
                if (error!=null)
                    System.Diagnostics.Debug.WriteLine(error);
            }
            catch (Exception e)
            {
                ServicesLocation.ExceptionGuard.OnException(e);
            }
        }

        static void OnTaskRun()
        {
            System.Diagnostics.Debug.WriteLine("BGTask run"); 
            var queryCommandExecutor = Mvx.IoCProvider.Resolve<QueryCommandExecutor>();
            var settings = Mvx.IoCProvider.Resolve<SettingsService>();
            var remainingNotificationResponse = queryCommandExecutor.Execute(new NotificationsForFinancesDatabaseCommand(
                settings.GetNotificationSettings()
            )).Result;

            if (remainingNotificationResponse.IsSuccess)
            {
                DispatchQueue.MainQueue.DispatchSync(() =>
                {
                    ShowNotification(remainingNotificationResponse.Results);
                });
            } 
        }
        
        
        static string GetNotificationTitle(FinanceOperation forOperation)
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

        static string GetNotificationContent(FinanceOperation forOperation)
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

        static void ShowNotification(FinanceOperation operation)
        {
            var center = UNUserNotificationCenter.Current;

            var content = new UNMutableNotificationContent();
            content.Title = GetNotificationTitle(operation);
            content.Body = GetNotificationContent(operation);
            var userInfo = new NSMutableDictionary
            {
                { new NSString("data"), new NSString(JsonConvert.SerializeObject(operation, new JsonSerializerSettings() {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
                ) }
            };
            
            content.UserInfo = userInfo;
            content.Sound = UNNotificationSound.Default;
            
            var request = UNNotificationRequest.FromIdentifier(Guid.NewGuid().ToString("N"), content, null);
            center.AddNotificationRequest(request, error =>
            {
                if (error!=null)
                    System.Diagnostics.Debug.WriteLine("Notification send error: " + error);
            });
        }
    }
}