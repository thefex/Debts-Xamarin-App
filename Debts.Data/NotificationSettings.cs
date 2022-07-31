using System;

namespace Debts.Data
{
    public class NotificationSettings
    {
        public TimeSpan MinimalAmountOfTimeBeforeUpcomingNotifications { get; set; }
        
        public TimeSpan MinimalAmountOfTimeAfterDeadlineExceedNotifications { get; set; }
        
        public bool UpcomingDebtsNotificationsEnabled { get; set; }
        
        public bool UpcomingLoansNotificationsEnabled { get; set; }
        
        public bool UnpaidDebtsNotificationsEnabled { get; set; }
        
        public bool UnpaidLoansNotificationsEnabled { get; set; }

        public int MinimalTimeAmountInSecondsBetweenNotificationRequest { get; set; } = 30;
    }
}