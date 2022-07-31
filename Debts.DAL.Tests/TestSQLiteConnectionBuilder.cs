using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Debts.Commands.Contacts;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Contacts;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.ViewModel.Contacts;
using SQLite;

namespace Debts.DAL.Tests
{
    public class TestSQLiteConnectionBuilder
    {
        public async Task<QueryCommandExecutor> GetCommandQueryExecutor()
        {
            var connection = new SQLiteAsyncConnection("DataSource=:memory:");

            try
            {
                await connection.Table<ContactDetails>().DeleteAsync(x => true);
                await connection.Table<FinanceOperation>().DeleteAsync(x => true);
            }
            catch (Exception)
            {
                
            }

            return new QueryCommandExecutor(connection);
        }

       
        public async Task SetupMockedFinanceOperationsForNotifications(QueryCommandExecutor commandExecutor,
            int numberOfUnpaidDebts, int numberOfUnpaidLoans, int numberOfUpcomingDebts, int numberOfUpcomingLoans, int numberOfPaidOperations,
            TimeSpan timeAmountBeforeExpiredNotificationSent, TimeSpan timeAmountBeforeUpcomingNotificationSent)
        {
            var contact = await commandExecutor.Execute(new AddNewContactCommandQuery("John Test"));
            
            var list = new List<FinanceOperation>();
            
            for (int i = 0; i < numberOfUnpaidDebts; ++i)
            {
                FinanceOperation operation = new FinanceOperation()
                {
                    Title = "Test " + i,
                    RelatedTo = contact,
                    AssignedContactId = contact.ContactPrimaryId,
                    Type = FinanceOperationType.Debt,
                    PaymentDetails = new PaymentDetails()
                    {
                        DeadlineDate = DateTime.UtcNow.Subtract(timeAmountBeforeExpiredNotificationSent.Add(TimeSpan.FromMinutes(10))).AddSeconds(i+1)
                    }
                };

                await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(operation));
                
                list.Add(operation);
            }
            
            for (int i = 0; i < numberOfUnpaidLoans; ++i)
            {
                FinanceOperation operation = new FinanceOperation()
                {
                    Title = "Test " + i,
                    RelatedTo = contact,
                    AssignedContactId = contact.ContactPrimaryId,
                    Type = FinanceOperationType.Loan,
                    PaymentDetails = new PaymentDetails()
                    {
                        DeadlineDate = DateTime.UtcNow.Subtract(timeAmountBeforeExpiredNotificationSent.Add(TimeSpan.FromMinutes(10))).AddSeconds(i+1)
                    }
                };

                await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(operation));
                
                list.Add(operation);
            }
            
            for (int i = 0; i < numberOfUpcomingDebts; ++i)
            {
                FinanceOperation operation = new FinanceOperation()
                {
                    Title = "Test " + i,
                    RelatedTo = contact,
                    AssignedContactId = contact.ContactPrimaryId,
                    Type = FinanceOperationType.Debt,
                    PaymentDetails = new PaymentDetails()
                    {
                        DeadlineDate = DateTime.UtcNow.Add(timeAmountBeforeUpcomingNotificationSent.Subtract(TimeSpan.FromMinutes(10))).AddSeconds(i+1)
                    }
                };

                await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(operation));
                list.Add(operation);
            }
            
            for (int i = 0; i < numberOfUpcomingLoans; ++i)
            {
                FinanceOperation operation = new FinanceOperation()
                {
                    Title = "Test " + i,
                    RelatedTo = contact,
                    AssignedContactId = contact.ContactPrimaryId,
                    Type = FinanceOperationType.Loan,
                    PaymentDetails = new PaymentDetails()
                    {
                        DeadlineDate = DateTime.UtcNow.Add(timeAmountBeforeUpcomingNotificationSent.Subtract(TimeSpan.FromMinutes(10))).AddSeconds(i+1)
                    }
                };

                await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(operation));
                list.Add(operation);
            }

            for (int i = 0; i < numberOfPaidOperations; ++i)
            {
                FinanceOperation operation = new FinanceOperation()
                {
                    Title = "Test " + i,
                    RelatedTo = contact,
                    AssignedContactId = contact.ContactPrimaryId,
                    Type = FinanceOperationType.Loan,
                    PaymentDetails = new PaymentDetails()
                    {
                        DeadlineDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(5)),
                        PaymentDate = DateTime.UtcNow
                    }
                };

                await commandExecutor.Execute(new AddNewFinanceOperationCommandQuery(operation));
                list.Add(operation);
            }

        }
    }
}