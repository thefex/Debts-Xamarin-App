using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer.Budget;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Debts.DataAccessLayer
{
    public class QueryCommandExecutor 
    {
        private readonly SQLiteAsyncConnection _connection;
        private static bool _isInitialized;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        
        public QueryCommandExecutor(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        public async Task InvokeQuery(Func<SQLiteAsyncConnection, Task> query)
        {
            try
            {
                await _semaphore.WaitAsync();

                if (!_isInitialized)
                {
                    await InitializeDatabaseIfRequired();
                    _isInitialized = true;
                }

                await query(_connection);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        public async Task<TResult> Execute<TResult>(IDataQuery<TResult> dataQuery)
        {
            try
            {
                await _semaphore.WaitAsync();
                if (!_isInitialized)
                {
                    await InitializeDatabaseIfRequired();
                    _isInitialized = true;
                }
                
                return await dataQuery.Query(_connection);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Execute(IDataCommand dataCommand)
        {
            try
            {
                await _semaphore.WaitAsync();
                if (!_isInitialized)
                {
                    await InitializeDatabaseIfRequired();
                    _isInitialized = true;
                }
                
                await dataCommand.ExecuteCommand(_connection);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task InitializeDatabaseIfRequired()
        { 
            await _connection.CreateTablesAsync<FinanceOperation, ContactDetails, Note, PaymentDetails>();
            await _connection.CreateTableAsync<AppSchemaVersionData>();

            var appSchema = await _connection.Table<AppSchemaVersionData>().ToListAsync();
            
            if (appSchema.All(x => x.Name != "budget"))
                await _connection.DropTableAsync<BudgetCategory>();
            
            await _connection.CreateTablesAsync<BudgetCategory, BudgetItem>();
            await new InitializeBudgetCategoriesCommand().ExecuteCommand(_connection);
            /**      for (int i = 0; i < 1000; ++i)
                  {
                      var contact = new ContactDetails()
                      {
                          FirstName = "Test",
                          LastName = i.ToString(),
                          PhoneNumber = string.Empty,
                      };
                      await _connection.InsertAsync(contact);
                      
                      var finance = new FinanceOperation()
                      {
                          RelatedTo = contact,
                          Title = "test",
                          PaymentDetails = new PaymentDetails()
                          {
                              DeadlineDate = i %2 == 0 ? DateTime.UtcNow.AddHours(i*12) : DateTime.UtcNow.Subtract(TimeSpan.FromHours(i*12+1)),
                              PaymentDate = i%4 == 0 ?  null : new Nullable<DateTime>(DateTime.UtcNow.Subtract(TimeSpan.FromHours(i*12+1))),
                              Amount = i%999,
                              Currency = "PLN"
                          }
                      };
                      await _connection.InsertWithChildrenAsync(finance);
      
                      var note = new Note()
                      {
                          Type = NoteType.Default,
                          Text = i.ToString(),
                          AssignedToFinanceOperationId = finance.FinancePrimaryId,
                          CreatedAt = DateTime.UtcNow
                      };
                      await _connection.InsertAsync(note);
                  }*/
        } 
    }
}