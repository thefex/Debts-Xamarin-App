using System;
using System.Threading.Tasks;
using Debts.Commands.FinanceDetails;
using Debts.Commands.Finances;
using Debts.Data;
using Debts.DataAccessLayer.Contacts;
using Debts.DataAccessLayer.FinanceOperations;
using NUnit.Framework;

namespace Debts.DAL.Tests
{
    [TestFixture]
    public class NotificationsForFinancesDatabaseCommandTests
    {
        private TestSQLiteConnectionBuilder _connectionBuilder = new TestSQLiteConnectionBuilder();
        
        [Test]
        public async Task GetNotification_WhenDatabaseEmpty_AllNotificationsEnabled_ShouldReturnNothing()
        {
            var settings = BuildNotificationSettingsWhenEverythingEnabled();
            var command = new NotificationsForFinancesDatabaseCommand(settings);

            var results = await (await _connectionBuilder.GetCommandQueryExecutor()).Execute(command);
            
            Assert.IsFalse(results.IsSuccess);
        }
        
        [Test]
        public async Task GetNotification_WhenDatabaseContainsUpcomingDebt_SettingsUpcomingDebt_ShouldReturnUpcomingDebt()
        {
            var settings = BuildNotificationSettingsWhenEverythingEnabled();
            settings.UpcomingLoansNotificationsEnabled = false;
            settings.UpcomingDebtsNotificationsEnabled = true;
            settings.UnpaidDebtsNotificationsEnabled = false;
            settings.UnpaidLoansNotificationsEnabled = false;
            
            var command = new NotificationsForFinancesDatabaseCommand(settings);
            var commandQueryExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            await _connectionBuilder.SetupMockedFinanceOperationsForNotifications(commandQueryExecutor, 2, 2, 2, 2, 5,
                settings.MinimalAmountOfTimeAfterDeadlineExceedNotifications, settings.MinimalAmountOfTimeBeforeUpcomingNotifications);
            
            var results = await commandQueryExecutor.Execute(command);
            
            Assert.IsTrue(results.IsSuccess, "Expected returned operation");
            Assert.IsTrue(results.Results.Type == FinanceOperationType.Debt, "Expected debt");
            Assert.IsNull(results.Results.PaymentDetails.PaymentDate, "Expected not paid operation");
            Assert.IsTrue(results.Results.PaymentDetails.DeadlineDate > DateTime.UtcNow, "Expected upcoming operation");
        }
        
        [Test]
        public async Task GetNotification_WhenDatabaseContainsUpcomingLoan_SettingsUpcomingLoan_ShouldReturnUpcomingLoan()
        {
            var settings = BuildNotificationSettingsWhenEverythingEnabled();
            settings.UpcomingLoansNotificationsEnabled = true;
            settings.UpcomingDebtsNotificationsEnabled = false;
            settings.UnpaidDebtsNotificationsEnabled = false;
            settings.UnpaidLoansNotificationsEnabled = false;
            
            var command = new NotificationsForFinancesDatabaseCommand(settings);
            var commandQueryExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            await _connectionBuilder.SetupMockedFinanceOperationsForNotifications(commandQueryExecutor, 2, 2, 2, 2,
                5,
                settings.MinimalAmountOfTimeAfterDeadlineExceedNotifications, settings.MinimalAmountOfTimeBeforeUpcomingNotifications);
            
            var results = await commandQueryExecutor.Execute(command);
            
            Assert.IsTrue(results.IsSuccess, "Expected returned operation");
            Assert.IsTrue(results.Results.Type == FinanceOperationType.Loan, "Expected loan");
            Assert.IsNull(results.Results.PaymentDetails.PaymentDate, "Expected not paid operation");
            Assert.IsTrue(results.Results.PaymentDetails.DeadlineDate > DateTime.UtcNow, "Expected upcoming operation");
        }
        
        [Test]
        public async Task GetNotification_WhenDatabaseContainsUnpaidDebt_SettingsUnpaidDebt_ShouldReturnUnpaidDebt()
        {
            var settings = BuildNotificationSettingsWhenEverythingEnabled();
            settings.UpcomingLoansNotificationsEnabled = false;
            settings.UpcomingDebtsNotificationsEnabled = false;
            settings.UnpaidDebtsNotificationsEnabled = true;
            settings.UnpaidLoansNotificationsEnabled = false;
            
            var command = new NotificationsForFinancesDatabaseCommand(settings);
            var commandQueryExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            await _connectionBuilder.SetupMockedFinanceOperationsForNotifications(commandQueryExecutor, 2, 2, 2, 2, 5,
                settings.MinimalAmountOfTimeAfterDeadlineExceedNotifications, settings.MinimalAmountOfTimeBeforeUpcomingNotifications);
            
            var results = await commandQueryExecutor.Execute(command);
            
            Assert.IsTrue(results.IsSuccess, "Expected returned operation");
            Assert.IsTrue(results.Results.Type == FinanceOperationType.Debt, "Expected debt");
            Assert.IsNull(results.Results.PaymentDetails.PaymentDate, "Expected not paid operation");
            Assert.IsTrue(results.Results.PaymentDetails.DeadlineDate <= DateTime.UtcNow, "Expected expired operation");
        }
        
        [Test]
        public async Task GetNotification_WhenDatabaseContainsUnpaidLoan_SettingsUnpaidLoan_ShouldReturnUnpaidLoan()
        {
            var settings = BuildNotificationSettingsWhenEverythingEnabled();
            settings.UpcomingLoansNotificationsEnabled = false;
            settings.UpcomingDebtsNotificationsEnabled = false;
            settings.UnpaidDebtsNotificationsEnabled = false;
            settings.UnpaidLoansNotificationsEnabled = true;
            
            var command = new NotificationsForFinancesDatabaseCommand(settings);
            var commandQueryExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            await _connectionBuilder.SetupMockedFinanceOperationsForNotifications(commandQueryExecutor, 2, 2, 2, 2, 5,
                settings.MinimalAmountOfTimeAfterDeadlineExceedNotifications, settings.MinimalAmountOfTimeBeforeUpcomingNotifications);
            
            var results = await commandQueryExecutor.Execute(command);
            
            Assert.IsTrue(results.IsSuccess, "Expected returned operation");
            Assert.IsTrue(results.Results.Type == FinanceOperationType.Loan, "Expected loan");
            Assert.IsNull(results.Results.PaymentDetails.PaymentDate, "Expected not paid operation");
            Assert.IsTrue(results.Results.PaymentDetails.DeadlineDate <= DateTime.UtcNow, "Expected expired operation");
        }

        [Test]
        public async Task GetNotification_WhenMoreThanOneNotificationExist_ShouldNotReturnSameNotificationOnSecondRequest()
        {
            var settings = BuildNotificationSettingsWhenEverythingEnabled(); 
            
            var command = new NotificationsForFinancesDatabaseCommand(settings);
            var commandQueryExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            await _connectionBuilder.SetupMockedFinanceOperationsForNotifications(commandQueryExecutor, 2, 2, 2, 2, 5,
                settings.MinimalAmountOfTimeAfterDeadlineExceedNotifications, settings.MinimalAmountOfTimeBeforeUpcomingNotifications);
            
            var firstResults = await commandQueryExecutor.Execute(command);
            var secondResults = await commandQueryExecutor.Execute(command);
            
            Assert.IsTrue(firstResults.IsSuccess, "First call - Expected returned operation");
            Assert.IsTrue(secondResults.IsSuccess, "Second call - Expected returned operation");
            Assert.AreNotEqual(firstResults.Results, secondResults.Results, "Returned operations are not different!");
        }
        
        [Test]
        public async Task GetNotification_WhenOneNotificationExist_ShouldNotReturnNotificationOnSecondRequestBefore24HourPass()
        {
            var commandExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            var contact = await commandExecutor.Execute(new AddNewContactCommandQuery("Test contact"));
 
            FinanceOperation financeOperation = new FinanceOperation()
            {
                Title = "test",
                Type = FinanceOperationType.Loan,
                PaymentDetails = new PaymentDetails()
                {
                    DeadlineDate = DateTime.UtcNow.AddDays(2)
                },
                RelatedTo = contact,
                AssignedContactId = contact.ContactPrimaryId
            };

            await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(financeOperation));

            var settings = BuildNotificationSettingsWhenEverythingEnabled(); 
            
            var command = new NotificationsForFinancesDatabaseCommand(settings);
          
            var firstResults = await commandExecutor.Execute(command);
            var secondResults = await commandExecutor.Execute(command);
            
            Assert.IsTrue(firstResults.IsSuccess, "First call - Expected returned operation");
            Assert.IsFalse(secondResults.IsSuccess, "Second call - Did not expect operation");
        }

        [Test]
        public async Task GetNotification_WhenTwoNotificationsExist_ShouldReturnNotificationWithLeastTimeAmountToDeadline()
        {
            var commandExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            var contact = await commandExecutor.Execute(new AddNewContactCommandQuery("Test contact"));

            var notificationSettings = BuildNotificationSettingsWhenEverythingEnabled();
            notificationSettings.MinimalAmountOfTimeAfterDeadlineExceedNotifications = TimeSpan.FromMinutes(20);

            FinanceOperation debtWithLongUpcomingTime = new FinanceOperation()
            {
                Title = "test1",
                Type = FinanceOperationType.Debt,
                PaymentDetails = new PaymentDetails()
                {
                    DeadlineDate = DateTime.UtcNow.AddMinutes(60)
                },
                RelatedTo = contact,
                AssignedContactId = contact.ContactPrimaryId
            }; 
            FinanceOperation debtWithShortUpcomingTime = new FinanceOperation()
            {
                Title = "test2",
                Type = FinanceOperationType.Debt,
                PaymentDetails = new PaymentDetails()
                {
                    DeadlineDate = DateTime.UtcNow.AddMinutes(30)
                },
                RelatedTo = contact,
                AssignedContactId = contact.ContactPrimaryId
            };
            
            FinanceOperation loanWithExpiredTimeAfterShortUpcomingTimeDebt = new FinanceOperation()
            {
                Title = "test3",
                Type = FinanceOperationType.Loan,
                PaymentDetails = new PaymentDetails()
                {
                    DeadlineDate = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(45))
                },
                RelatedTo = contact,
                AssignedContactId = contact.ContactPrimaryId
            };
            await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(debtWithShortUpcomingTime));
            await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(debtWithLongUpcomingTime));
            await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(loanWithExpiredTimeAfterShortUpcomingTimeDebt));
            
            var command = new NotificationsForFinancesDatabaseCommand(notificationSettings);
            var firstNotificationResponse = await commandExecutor.Execute(command);
            var secondNotificationResponse = await commandExecutor.Execute(command);
            var thirdNotificationResponse = await commandExecutor.Execute(command);

            Assert.IsTrue(firstNotificationResponse.IsSuccess);
            Assert.AreEqual(firstNotificationResponse.Results, debtWithShortUpcomingTime);
            
            Assert.IsTrue(secondNotificationResponse.IsSuccess);
            Assert.AreEqual(secondNotificationResponse.Results, loanWithExpiredTimeAfterShortUpcomingTimeDebt);
            
            Assert.IsTrue(thirdNotificationResponse.IsSuccess);
            Assert.AreEqual(thirdNotificationResponse.Results, debtWithLongUpcomingTime);
        }

        [Test]
        public async Task GetNotification_UpcomingDebtWithDeadlineMoreThanMinimalAmountOfTimeBeforeUpcomingNotification_ShouldNotReturnNotification()
        {
            var commandExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            var contact = await commandExecutor.Execute(new AddNewContactCommandQuery("Test contact"));

            var notificationSettings = BuildNotificationSettingsWhenEverythingEnabled();
            notificationSettings.MinimalAmountOfTimeBeforeUpcomingNotifications = TimeSpan.FromDays(3);

            FinanceOperation financeOperation = new FinanceOperation()
            {
                Title = "test",
                Type = FinanceOperationType.Debt,
                PaymentDetails = new PaymentDetails()
                {
                    DeadlineDate = DateTime.UtcNow.AddDays(4)
                },
                RelatedTo = contact,
                AssignedContactId = contact.ContactPrimaryId
            };

            await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(financeOperation));
            
            var command = new NotificationsForFinancesDatabaseCommand(notificationSettings);
            var notificationResponse = await commandExecutor.Execute(command);
            
            Assert.IsFalse(notificationResponse.IsSuccess);
        }
        
        [Test]
        public async Task GetNotification_UpcomingLoanWithDeadlineMoreThanMinimalAmountOfTimeBeforeUpcomingNotification_ShouldNotReturnNotification()
        {
            var commandExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            var contact = await commandExecutor.Execute(new AddNewContactCommandQuery("Test contact"));

            var notificationSettings = BuildNotificationSettingsWhenEverythingEnabled();
            notificationSettings.MinimalAmountOfTimeBeforeUpcomingNotifications = TimeSpan.FromDays(3);

            FinanceOperation financeOperation = new FinanceOperation()
            {
                Title = "test",
                Type = FinanceOperationType.Loan,
                PaymentDetails = new PaymentDetails()
                {
                    DeadlineDate = DateTime.UtcNow.AddDays(4)
                },
                RelatedTo = contact,
                AssignedContactId = contact.ContactPrimaryId
            };

            await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(financeOperation));
            
            var command = new NotificationsForFinancesDatabaseCommand(notificationSettings);
            var notificationResponse = await commandExecutor.Execute(command);
            
            Assert.IsFalse(notificationResponse.IsSuccess);
        }
        
        [Test]
        public async Task GetNotification_UnpaidDebtWithExpiredDeadlineLessThanThanMinimalAmountOfTimAfterUnpaidNotification_ShouldNotReturnNotification()
        {
            var commandExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            var contact = await commandExecutor.Execute(new AddNewContactCommandQuery("Test contact"));

            var notificationSettings = BuildNotificationSettingsWhenEverythingEnabled();
            notificationSettings.MinimalAmountOfTimeAfterDeadlineExceedNotifications = TimeSpan.FromDays(3);

            FinanceOperation financeOperation = new FinanceOperation()
            {
                Title = "test",
                Type = FinanceOperationType.Debt,
                PaymentDetails = new PaymentDetails()
                {
                    DeadlineDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(2))
                },
                RelatedTo = contact,
                AssignedContactId = contact.ContactPrimaryId
            };

            await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(financeOperation));
            
            var command = new NotificationsForFinancesDatabaseCommand(notificationSettings);
            var notificationResponse = await commandExecutor.Execute(command);
            
            Assert.IsFalse(notificationResponse.IsSuccess);
        }

        [Test]
        public async Task GetNotification_UnpaidLoanWithExpiredDeadlineLessThanMinimalAmountOfTimeAfterUnpaidNotification_ShouldNotReturnNotification()
        {
            var commandExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            var contact = await commandExecutor.Execute(new AddNewContactCommandQuery("Test contact"));

            var notificationSettings = BuildNotificationSettingsWhenEverythingEnabled();
            notificationSettings.MinimalAmountOfTimeAfterDeadlineExceedNotifications = TimeSpan.FromDays(3);

            FinanceOperation financeOperation = new FinanceOperation()
            {
                Title = "test",
                Type = FinanceOperationType.Loan,
                PaymentDetails = new PaymentDetails()
                {
                    DeadlineDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(2))
                },
                RelatedTo = contact,
                AssignedContactId = contact.ContactPrimaryId
            };

            await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(financeOperation));
            
            var command = new NotificationsForFinancesDatabaseCommand(notificationSettings);
            var notificationResponse = await commandExecutor.Execute(command);
            
            Assert.IsFalse(notificationResponse.IsSuccess);
        }

        [Test]
        public async Task GetNotification_WhenRecentSentNotificationDateIsLessThan24Hours_ShouldNotReturnNotification()
        {
            var commandExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            var contact = await commandExecutor.Execute(new AddNewContactCommandQuery("Test contact"));

            var notificationSettings = BuildNotificationSettingsWhenEverythingEnabled();
            notificationSettings.MinimalAmountOfTimeAfterDeadlineExceedNotifications = TimeSpan.FromDays(3);

            FinanceOperation financeOperation = new FinanceOperation()
            {
                Title = "test",
                Type = FinanceOperationType.Loan,
                PaymentDetails = new PaymentDetails()
                {
                    DeadlineDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(4))
                },
                RecentNotificationSentDate = DateTime.UtcNow.Subtract(TimeSpan.FromHours(12)),
                RelatedTo = contact,
                AssignedContactId = contact.ContactPrimaryId
            };

            await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(financeOperation));
            
            
            var command = new NotificationsForFinancesDatabaseCommand(notificationSettings);
            var notificationResponse = await commandExecutor.Execute(command);
            
            Assert.IsFalse(notificationResponse.IsSuccess);
        }
        
        [Test]
        public async Task GetNotification_WhenRecentSentNotificationDateIsMoreThan24Hours_ShouldNotReturnNotification()
        {
            var commandExecutor = await _connectionBuilder.GetCommandQueryExecutor();
            var contact = await commandExecutor.Execute(new AddNewContactCommandQuery("Test contact"));

            var notificationSettings = BuildNotificationSettingsWhenEverythingEnabled();
            notificationSettings.MinimalAmountOfTimeAfterDeadlineExceedNotifications = TimeSpan.FromDays(3);

            FinanceOperation financeOperation = new FinanceOperation()
            {
                Title = "test",
                Type = FinanceOperationType.Loan,
                PaymentDetails = new PaymentDetails()
                {
                    DeadlineDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(4))
                },
                RecentNotificationSentDate = DateTime.UtcNow.Subtract(TimeSpan.FromHours(25)),
                RelatedTo = contact,
                AssignedContactId = contact.ContactPrimaryId
            };

            await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(financeOperation));
            
            
            var command = new NotificationsForFinancesDatabaseCommand(notificationSettings);
            var notificationResponse = await commandExecutor.Execute(command);
            
            Assert.IsFalse(notificationResponse.IsSuccess);
        }
        
        NotificationSettings BuildNotificationSettingsWhenEverythingEnabled()
        {
            return new NotificationSettings()
            {
                UpcomingDebtsNotificationsEnabled = true,
                UnpaidDebtsNotificationsEnabled = true,
                UpcomingLoansNotificationsEnabled = true,
                UnpaidLoansNotificationsEnabled = true,
                MinimalAmountOfTimeBeforeUpcomingNotifications = TimeSpan.FromDays(3),
                MinimalAmountOfTimeAfterDeadlineExceedNotifications = TimeSpan.FromDays(3),
                MinimalTimeAmountInSecondsBetweenNotificationRequest = -1
            };
        } 
    }
}