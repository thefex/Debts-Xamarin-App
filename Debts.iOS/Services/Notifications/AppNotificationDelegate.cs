using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UserNotifications;

namespace Debts.iOS.Services.Notifications
{
    class AppNotificationDelegate : UNUserNotificationCenterDelegate
    {
        public AppNotificationDelegate()
        {
        }

        protected AppNotificationDelegate(NSObjectFlag t) : base(t)
        {
        }

        protected internal AppNotificationDelegate(IntPtr handle) : base(handle)
        {
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound);
        }

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response,
            Action completionHandler)
        {
            if (response.IsDefaultAction)// && (UIApplication.SharedApplication?.ApplicationState ?? UIApplicationState.Inactive) == UIApplicationState.Active )
            {
                var data = response.Notification.Request.Content
                    .UserInfo
                    .ToDictionary<KeyValuePair<NSObject, NSObject>, string, object>(item => (NSString)item.Key, item => item.Value.ToString());

                new NotificationTappedHandler().OnTapped(data);
            }
            
            completionHandler();
        }
    }
}