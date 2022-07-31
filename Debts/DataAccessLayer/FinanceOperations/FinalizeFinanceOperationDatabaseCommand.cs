using System;
using System.Threading.Tasks;
using Debts.Data;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Debts.DataAccessLayer.FinanceOperations
{
    public class FinalizeFinanceOperationDatabaseCommand : IDataQuery<PaymentDetails>
    {
        private readonly FinanceOperation _financeOperation;

        public FinalizeFinanceOperationDatabaseCommand(FinanceOperation financeOperation)
        {
            _financeOperation = financeOperation;
        }
        
        public async Task<PaymentDetails> Query(SQLiteAsyncConnection connection)
        {
            _financeOperation.PaymentDetails.PaymentDate = DateTime.UtcNow;
            await connection.UpdateWithChildrenAsync(_financeOperation.PaymentDetails);
            return _financeOperation.PaymentDetails;
        }
    }
}