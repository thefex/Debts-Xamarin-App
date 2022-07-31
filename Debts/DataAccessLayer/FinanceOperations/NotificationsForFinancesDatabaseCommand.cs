using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Model;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Debts.DataAccessLayer.FinanceOperations
{
    public class NotificationsForFinancesDatabaseCommand : IDataQuery<LayerResponse<FinanceOperation>>
    {
        private readonly NotificationSettings _notificationSettings;

        public NotificationsForFinancesDatabaseCommand(NotificationSettings notificationSettings)
        {
            _notificationSettings = notificationSettings;
        }
        
        public async Task<LayerResponse<FinanceOperation>> Query(SQLiteAsyncConnection connection)
        {
            if (await HasAnyNotificationBeenSentRecently(connection))
                return new LayerResponse<FinanceOperation>().AddErrorMessage("recently_sent_notification");
            
            if (!_notificationSettings.UnpaidDebtsNotificationsEnabled &&
                !_notificationSettings.UnpaidLoansNotificationsEnabled &&
                !_notificationSettings.UpcomingDebtsNotificationsEnabled &&
                !_notificationSettings.UpcomingLoansNotificationsEnabled)
                return new LayerResponse<FinanceOperation>().AddErrorMessage("notifications_disabled");

            var financeOperationToNotify = await GetFinanceOperationToNotify(connection);
            
            if (financeOperationToNotify == null)
                return new LayerResponse<FinanceOperation>().AddErrorMessage("no_operation_for_notifications");
                
            await UpdateRecentNotificationDate(financeOperationToNotify, connection);
            return new LayerResponse<FinanceOperation>(financeOperationToNotify);
        }

        async Task<bool> HasAnyNotificationBeenSentRecently(SQLiteAsyncConnection sqLiteAsyncConnection)
        {
            var recentlyNotifiedFinanceOperation = await sqLiteAsyncConnection.Table<FinanceOperation>()
                .Where(x => x.RecentNotificationSentDate != null)
                .OrderByDescending(x => x.RecentNotificationSentDate)
                .FirstOrDefaultAsync();

            return recentlyNotifiedFinanceOperation != null &&
                   recentlyNotifiedFinanceOperation.RecentNotificationSentDate.HasValue &&
                   (DateTime.UtcNow - recentlyNotifiedFinanceOperation.RecentNotificationSentDate.Value).TotalSeconds <= _notificationSettings.MinimalTimeAmountInSecondsBetweenNotificationRequest;
        }

        async Task<FinanceOperation> GetFinanceOperationToNotify(SQLiteAsyncConnection sqLiteAsyncConnection)
        {
            string sqlQuery = "SELECT * " +
                              "FROM finance_operations AS operation " +
                              "JOIN payments AS payment ON operation.FinancePrimaryId = payment.AssignedToFinanceOperationId " +
                              "JOIN contacts AS contact ON operation.AssignedContactId = contact.ContactPrimaryId " +
                              "WHERE (operation.RecentNotificationSentDate IS NULL OR " +
                              "Cast ((JulianDay(?) - JulianDay(operation.RecentNotificationSentDate)) * 24 * 60 * 60 As Integer) >= 24*60*60" + // minimum one day before repeating notification 
                              ")  " +
                              "AND (";

            bool shouldAddOr = false;
            List<object> parameterList = new List<object>();
            parameterList.Add(DateTime.UtcNow);

            if (_notificationSettings.UpcomingDebtsNotificationsEnabled)
            {
                sqlQuery += "(operation.Type = 1 AND payment.DeadlineDate > ? AND payment.DeadlineDate <= ? AND payment.PaymentDate IS NULL) ";
                shouldAddOr = true;
                
                parameterList.Add(DateTime.UtcNow);
                parameterList.Add(DateTime.UtcNow + _notificationSettings.MinimalAmountOfTimeBeforeUpcomingNotifications);
            }

            if (_notificationSettings.UpcomingLoansNotificationsEnabled)
            {
                if (shouldAddOr)
                    sqlQuery += "OR ";
                sqlQuery += "(operation.Type = 0 AND payment.DeadlineDate > ? AND payment.DeadlineDate <= ? AND payment.PaymentDate IS NULL) ";
                shouldAddOr = true;
                
                parameterList.Add(DateTime.UtcNow);
                parameterList.Add(DateTime.UtcNow + _notificationSettings.MinimalAmountOfTimeBeforeUpcomingNotifications);
            }

            if (_notificationSettings.UnpaidDebtsNotificationsEnabled)
            {
                if (shouldAddOr)
                    sqlQuery += "OR ";
                sqlQuery += "(operation.Type = 1 AND payment.DeadlineDate < ? AND payment.PaymentDate IS NULL)";
                shouldAddOr = true;
                
                parameterList.Add(DateTime.UtcNow - _notificationSettings.MinimalAmountOfTimeAfterDeadlineExceedNotifications);
            }
            
            if (_notificationSettings.UnpaidLoansNotificationsEnabled)
            {
                if (shouldAddOr)
                    sqlQuery += "OR ";
                sqlQuery += "(operation.Type = 0 AND payment.DeadlineDate < ? AND payment.PaymentDate IS NULL)";
                shouldAddOr = true;
                
                parameterList.Add(DateTime.UtcNow - _notificationSettings.MinimalAmountOfTimeAfterDeadlineExceedNotifications);
            }

            sqlQuery += ") " +
                        "ORDER BY " +
                        "operation.RecentNotificationSentDate DESC, " +
                        "(ABS(payment.DeadlineDate - ?)) ASC " +
                        "LIMIT 0,1";
            
            parameterList.Add(DateTime.UtcNow);

            var results = (await sqLiteAsyncConnection.QueryAsync<FinanceOperation>(sqlQuery, parameterList.ToArray()));
            var financeOperationToNotify = results.FirstOrDefault();
            if (financeOperationToNotify != null)
                await sqLiteAsyncConnection.GetChildrenAsync(financeOperationToNotify, true);
            
            return financeOperationToNotify;
        }

        async Task UpdateRecentNotificationDate(FinanceOperation operation, SQLiteAsyncConnection connection)
        {
            operation.RecentNotificationSentDate = DateTime.UtcNow;
            await connection.UpdateWithChildrenAsync(operation);
        }
    }
}