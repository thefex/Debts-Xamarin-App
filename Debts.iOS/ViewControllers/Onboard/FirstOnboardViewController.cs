using System;
using Debts.ViewModel.OnboardViewModel;
using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Onboard
{
    public class FirstOnboardViewController : OnboardPageViewController<FirstDebtsOnboardViewModel>
    {
        public FirstOnboardViewController()
        {
        }

        public FirstOnboardViewController(IntPtr handle) : base(handle)
        {
        }

        public FirstOnboardViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        protected override float ScaleFactor => 0.55f;
        protected override string AnimationName => "01_onboard";
    }
}