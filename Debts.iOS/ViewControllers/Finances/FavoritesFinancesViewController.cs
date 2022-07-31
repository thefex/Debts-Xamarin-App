using System;
using Debts.Resources;
using Debts.ViewModel.FinancesViewModel;
using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace Debts.iOS.ViewControllers.Finances
{
    [MvxPagePresentation]
    public class FavoritesFinancesViewController : FinancesViewController<FavoritesFinancesViewModel>
    {
        public FavoritesFinancesViewController()
        {
        }

        public FavoritesFinancesViewController(IntPtr handle) : base(handle)
        {
        }

        public FavoritesFinancesViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        public override string EmptyListText => TextResources.FavoriteFinanceList_EmptyListText;
        protected override string PageTitle => TextResources.FavoriteFinanceList_Title;
    }
}