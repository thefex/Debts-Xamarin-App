using Debts.Resources;

namespace Debts.ViewModel.OnboardViewModel
{
    public class ThirdGpsOnboardViewModel : OnboardViewModel
    {
        public override string Title => TextResources.Tutorial_GpsTracking_Title;
        public override string Description => TextResources.Tutorial_GpsTracking_Description;
    }
}