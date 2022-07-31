using System;
using Debts.ViewModel.OnboardViewModel;
using Foundation;
using UIKit;

namespace Debts.iOS.ViewControllers.Onboard
{
    public class SecondPhoneContactsOnboardViewController : OnboardPageViewController<SecondPhoneContactsOnboardViewModel>
    {
        public SecondPhoneContactsOnboardViewController()
        {
        }

        public SecondPhoneContactsOnboardViewController(IntPtr handle) : base(handle)
        {
        }

        public SecondPhoneContactsOnboardViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        protected override string AnimationName => "02_onboard";
    }
}