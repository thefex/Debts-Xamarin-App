using System;
using Debts.Resources;
using Debts.ViewModel.Finances;
using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace Debts.iOS.ViewControllers.Finances
{
    [MvxPagePresentation]
    public class LoansFinancesViewController : FinancesViewController<MyLoansViewModel>
    {
        public LoansFinancesViewController()
        {
        }

        public LoansFinancesViewController(IntPtr handle) : base(handle)
        {
        }

        public LoansFinancesViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        public override string EmptyListText => TextResources.MyLoans_EmptyListText;
        protected override string PageTitle => "Loans";
    }
}