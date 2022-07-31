using System;
using Debts.ViewModel.OnboardViewModel;
using Foundation;
using UIKit;

namespace Debts.iOS.ViewControllers.Onboard
{
    public class FourthNotifyDebtorViewController : OnboardPageViewController<FourthNotifyDebtorOnboardViewModel>
    {
        public FourthNotifyDebtorViewController()
        {
        }

        public FourthNotifyDebtorViewController(IntPtr handle) : base(handle)
        {
        }

        public FourthNotifyDebtorViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        protected override string AnimationName => "04_onboard";
    }
}