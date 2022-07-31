using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Services.Notifications;
using Debts.Services.Settings;
using MvvmCross;
using MvvmCross.Platforms.Android.Core;

namespace Debts.Droid.Services.Notifications
{
    public class AndroidNotificationsWorkScheduler : INotificationsWorkScheduler
    {
        
        public void StartRepeatedBackgroundJobs()
        {
             var periodicWork = PeriodicWorkRequest.Builder.From<SendNotificationsWorker>(TimeSpan.FromSeconds(5))
                .Build();

            WorkManager.Instance.EnqueueUniquePeriodicWork("notifications_scheduler", ExistingPeriodicWorkPolicy.Keep, periodicWork);
        }
    }

    public class SendNotificationsWorker : Worker
    {
        public SendNotificationsWorker(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SendNotificationsWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }

        public override Result DoWork()
        {
            var setup = MvxAndroidSetupSingleton.EnsureSingletonAvailable(ApplicationContext);
            setup.EnsureInitialized();

            var queryCommandExecutor = Mvx.IoCProvider.Resolve<QueryCommandExecutor>();
            var settings = Mvx.IoCProvider.Resolve<SettingsService>();
            var remainingNotificationResponse = queryCommandExecutor.Execute(new NotificationsForFinancesDatabaseCommand(
                    settings.GetNotificationSettings()
                )).Result;

            if (remainingNotificationResponse.IsSuccess)
                new DroidNotificationCompatBuilder().ShowNotification(Application.Context, remainingNotificationResponse.Results);

            return Result.InvokeSuccess();
        }
    }
}