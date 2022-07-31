using System.Threading.Tasks;
using Debts.Data;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Debts.DataAccessLayer.FinanceOperations
{
    public class ToggleFavouriteStateFinanceOperationDatabaseCommand : IDataQuery<bool>
    {
        private readonly FinanceOperation _financeOperation;

        public ToggleFavouriteStateFinanceOperationDatabaseCommand(FinanceOperation financeOperation)
        {
            _financeOperation = financeOperation;
        }
        
        public async Task<bool> Query(SQLiteAsyncConnection connection)
        {
            _financeOperation.IsFavourite = !_financeOperation.IsFavourite;
            await connection.UpdateWithChildrenAsync(_financeOperation);

            return _financeOperation.IsFavourite;
        }
    }
}