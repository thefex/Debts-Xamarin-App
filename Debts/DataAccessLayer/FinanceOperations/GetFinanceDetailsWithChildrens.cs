using System.Threading.Tasks;
using Debts.Data;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Debts.DataAccessLayer.FinanceOperations
{
    public class GetFinanceDetailsWithChildrensQuery : IDataQuery<FinanceOperation>
    {
        private readonly FinanceOperation _forOperation;

        public GetFinanceDetailsWithChildrensQuery(FinanceOperation forOperation)
        {
            _forOperation = forOperation;
        }
        public async Task<FinanceOperation> Query(SQLiteAsyncConnection connection)
        {
            await connection.GetChildrenAsync(_forOperation, true);
            return _forOperation;
        }
    }
}