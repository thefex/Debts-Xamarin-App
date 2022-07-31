using System;
using Debts.Resources;
using Debts.ViewModel.FinancesViewModel;
using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace Debts.iOS.ViewControllers.Finances
{
    [MvxPagePresentation]
    public class DebtsFinancesViewController : FinancesViewController<MyDebtsViewModel>
    {
        public DebtsFinancesViewController()
        {
        }

        public DebtsFinancesViewController(IntPtr handle) : base(handle)
        {
        }

        public DebtsFinancesViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        public override string EmptyListText => TextResources.MyDebts_EmptyListText;
        
        protected override string PageTitle => "Debts";
    }
}