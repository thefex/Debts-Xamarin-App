using System.Threading.Tasks;
using Debts.Data;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;
using SQLitePCL;

namespace Debts.DataAccessLayer.FinanceOperations
{
    public class AddNewFinanceOperationCommandQuery : IDataQuery<FinanceOperation>
    {
        private readonly FinanceOperation _financeOperation;

        public AddNewFinanceOperationCommandQuery(FinanceOperation financeOperation)
        {
            _financeOperation = financeOperation;
        }
        
        public async Task<FinanceOperation> Query(SQLiteAsyncConnection connection)
        { 
            await connection.RunInTransactionAsync(sqLiteConnection =>
            {
                _financeOperation.RelatedTo = _financeOperation.RelatedTo;
                _financeOperation.PaymentDetails = _financeOperation.PaymentDetails;
                 sqLiteConnection.InsertWithChildren(_financeOperation, true); 
            });
  
            return _financeOperation;
        }
    }
}