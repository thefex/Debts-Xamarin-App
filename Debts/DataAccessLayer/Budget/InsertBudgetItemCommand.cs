using System.Threading.Tasks;
using Debts.Data;
using SQLite;

namespace Debts.DataAccessLayer.Budget
{
    public class InsertBudgetItemCommand : IDataQuery<BudgetItem>
    {
        private readonly BudgetItem _budgetItem;

        public InsertBudgetItemCommand(BudgetItem budgetItem)
        {
            _budgetItem = budgetItem;
        }
        
        public async Task<BudgetItem> Query(SQLiteAsyncConnection connection)
        {
            await connection.InsertAsync(_budgetItem);
            return _budgetItem;
        }
    }
}