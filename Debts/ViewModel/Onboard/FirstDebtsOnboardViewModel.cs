using Debts.Resources;

namespace Debts.ViewModel.OnboardViewModel
{
    public class FirstDebtsOnboardViewModel : OnboardViewModel
    {
        public override string Title => TextResources.Tutorial_Debts_Title;
        public override string Description => TextResources.Tutorial_Debts_Description;
    }
}