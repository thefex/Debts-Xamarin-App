using System.Threading.Tasks;
using Debts.Data;
using SQLite;

namespace Debts.DataAccessLayer.FinanceOperations
{
    public class DeleteFinanceOperationDatabaseCommand : IDataCommand
    {
        private readonly FinanceOperation _financeOperation;

        public DeleteFinanceOperationDatabaseCommand(FinanceOperation financeOperation)
        {
            _financeOperation = financeOperation;
        }
        public Task ExecuteCommand(SQLiteAsyncConnection connection) => connection.Table<FinanceOperation>().DeleteAsync(x => x.FinancePrimaryId == _financeOperation.FinancePrimaryId);
    }
}