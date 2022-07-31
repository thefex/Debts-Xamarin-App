using Debts.Data.Queries;

namespace Debts.DataAccessLayer.Budget
{
    public class BudgetGetExpensesQuery : BudgetGetAllQuery
    {
        public BudgetGetExpensesQuery(BudgetItemsQueryParameter itemsQueryParameter) : base(itemsQueryParameter)
        {
        }
    }
}