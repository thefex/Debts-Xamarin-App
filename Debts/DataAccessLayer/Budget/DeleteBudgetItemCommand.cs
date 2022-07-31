using System.Threading.Tasks;
using Debts.Data;
using SQLite;

namespace Debts.DataAccessLayer.Budget
{
    public class DeleteBudgetItemCommand : IDataCommand
    {
        private readonly BudgetItem _budgetItem;

        public DeleteBudgetItemCommand(BudgetItem budgetItem)
        {
            _budgetItem = budgetItem;
        }
        
        public Task ExecuteCommand(SQLiteAsyncConnection connection) => connection.Table<BudgetItem>().DeleteAsync(x => x.Id == _budgetItem.Id);
    }
}