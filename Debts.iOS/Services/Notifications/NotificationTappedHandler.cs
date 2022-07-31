using System.Collections.Generic;
using Debts.Data;
using Debts.Services;
using Debts.Services.Notifications;
using Debts.ViewModel;
using Foundation;
using MvvmCross;
using Newtonsoft.Json;
using UIKit;

namespace Debts.iOS.Services.Notifications
{
    public class NotificationTappedHandler
    {
        public void OnTapped(Dictionary<string, object> notification)
        {
            if (notification.ContainsKey("data"))
            {
                var data = notification["data"].ToString();
                var financeOperation = JsonConvert.DeserializeObject<FinanceOperation>(data);

                Mvx.IoCProvider.Resolve<FinanceOperationNotificationHandler>().HandleNotification(financeOperation); 
            }
        }
    }
}