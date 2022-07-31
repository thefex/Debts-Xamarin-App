using System;
using Debts.ViewModel.OnboardViewModel;
using Foundation;
using UIKit;

namespace Debts.iOS.ViewControllers.Onboard
{
    public class FifthBudgetOnboardViewController : OnboardPageViewController<BudgetOnboardViewModel>
    {
        public FifthBudgetOnboardViewController()
        {
        }

        public FifthBudgetOnboardViewController(IntPtr handle) : base(handle)
        {
        }

        public FifthBudgetOnboardViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        protected override string AnimationName => "05_onboard";
    }
}