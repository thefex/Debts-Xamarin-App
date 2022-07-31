using System;
using Debts.ViewModel.OnboardViewModel;
using Foundation;
using UIKit;

namespace Debts.iOS.ViewControllers.Onboard
{
    public class SixOnboardViewController : OnboardPageViewController<SixMoreOnboardViewModel>
    {
        public SixOnboardViewController()
        {
        }

        public SixOnboardViewController(IntPtr handle) : base(handle)
        {
        }

        public SixOnboardViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        protected override string AnimationName => "06_onboard";
    }
}