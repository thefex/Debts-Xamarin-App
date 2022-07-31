using System;
using Debts.ViewModel.Budget;
using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Budget
{
    [MvxModalPresentation(Animated = true, ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen, ModalTransitionStyle = UIModalTransitionStyle.CoverVertical)]
    public class PickBudgetCategoryViewController : BasePickBudgetCategoryViewController<PickBudgetCategoryViewModel>
    {
        public PickBudgetCategoryViewController()
        {
        }

        public PickBudgetCategoryViewController(IntPtr handle) : base(handle)
        {
        }

        public PickBudgetCategoryViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }
    }
}