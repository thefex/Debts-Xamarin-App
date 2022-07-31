using Debts.DataAccessLayer;
using Debts.Messenging.Messages.App;
using Debts.Services;
using Debts.Services.AppGrowth;

namespace Debts.ViewModel.Budget
{
    public class PickBudgetCategoryForFilterViewModel : PickBudgetCategoryViewModel
    {
        public PickBudgetCategoryForFilterViewModel(QueryCommandExecutor queryCommandExecutor, PremiumService premiumService) : base(queryCommandExecutor, premiumService)
        {
        }
    }
}