using Debts.Data.Queries;

namespace Debts.DataAccessLayer.Budget
{
    public class BudgetGetIncomeQuery : BudgetGetAllQuery
    {
        public BudgetGetIncomeQuery(BudgetItemsQueryParameter itemsQueryParameter) : base(itemsQueryParameter)
        {
        }
    }
}