using System;
using Debts.ViewModel.OnboardViewModel;
using Foundation;
using UIKit;

namespace Debts.iOS.ViewControllers.Onboard
{
    public class ThirdGpsOnboardViewController : OnboardPageViewController<ThirdGpsOnboardViewModel>
    {
        public ThirdGpsOnboardViewController()
        {
        }

        public ThirdGpsOnboardViewController(IntPtr handle) : base(handle)
        {
        }

        public ThirdGpsOnboardViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        protected override string AnimationName => "03_onboard";
    }
}