using System;
using Debts.Resources;
using Debts.ViewModel.FinancesViewModel;
using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Finances
{
    [MvxPagePresentation]
    public class AllFinancesViewController : FinancesViewController<AllFinancesViewModel>
    {
        public AllFinancesViewController()
        {
        }

        public AllFinancesViewController(IntPtr handle) : base(handle)
        {
        }

        protected AllFinancesViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }
  
        public override string EmptyListText => TextResources.AllFinancesList_EmptyListText;
        protected override string PageTitle => TextResources.AllFinancesList_Title;
    }
}