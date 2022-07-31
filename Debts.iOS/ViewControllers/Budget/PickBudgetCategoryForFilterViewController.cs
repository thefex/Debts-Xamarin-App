using System;
using Debts.ViewModel.Budget;
using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Budget
{
    [MvxModalPresentation(Animated = true, ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen, ModalTransitionStyle = UIModalTransitionStyle.CoverVertical)]
    public class PickBudgetCategoryForFilterViewController : BasePickBudgetCategoryViewController<
        PickBudgetCategoryForFilterViewModel>
    {
        public PickBudgetCategoryForFilterViewController()
        {
        }

        public PickBudgetCategoryForFilterViewController(IntPtr handle) : base(handle)
        {
        }

        public PickBudgetCategoryForFilterViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }
    }
}