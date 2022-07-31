using Debts.Resources;

namespace Debts.ViewModel.OnboardViewModel
{
    public class BudgetOnboardViewModel : OnboardViewModel
    {
        public override string Title => TextResources.Tutorial_Budget_Title;
        public override string Description => TextResources.Tutorial_Budget_Description;
    }
}