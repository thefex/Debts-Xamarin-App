using Debts.Data;
using MvvmCross.AdvancedRecyclerView.Data.ItemUniqueIdProvider;

namespace Debts.Droid.Core
{
    public class BudgetCategoryItemUniqueIdProvider : IMvxItemUniqueIdProvider
    {
        public long GetUniqueId(object fromObject) => (fromObject as BudgetCategory).MainCategoryId;
    }
}