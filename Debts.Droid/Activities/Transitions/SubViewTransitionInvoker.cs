using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Support.Design.BottomAppBar;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Debts.Data;
using Debts.Droid.Fragments;
using Debts.Droid.Fragments.Budget;
using Debts.Droid.Fragments.Contacts;
using Debts.Droid.Fragments.Finances;
using Debts.Droid.Fragments.Main;
using Debts.Model;
using Debts.ViewModel;
using Java.IO;
using MvvmCross.Droid.Support.V7.Preference;
using MvvmCross.Platforms.Android.Views.Fragments;
using MvxDialogFragment = MvvmCross.Droid.Support.V4.MvxDialogFragment;
using MvxFragment = MvvmCross.Droid.Support.V4.MvxFragment;

namespace Debts.Droid.Activities.Transitions
{
    public class SubViewTransitionInvoker
    {
        private readonly Func<MainViewModel> _viewModelProvider;

        public SubViewTransitionInvoker(Func<MainViewModel> viewModelProvider)
        {
            _viewModelProvider = viewModelProvider;
        }

        private Drawable _navigationIcon;
        public void Initialize(BottomAppBar bottomAppBar, FloatingActionButton floatingActionButton)
        {
            AppBar = bottomAppBar;
            FloatingActionButton = floatingActionButton;
            _navigationIcon = bottomAppBar.NavigationIcon;
        }
        
        public BottomAppBar AppBar { get; private set; }
        
        public FloatingActionButton FloatingActionButton { get; private set; }
        
        public void OnSubViewAppeared(Fragment fragment)
        {
            SetCurrentSubPage(GetSubPage(fragment));
 
            if (fragment is FinanceDetailsFragment financeDetailsFragment)
                new FinanceDetailsTransitionSubViewAction(financeDetailsFragment).OnSubViewAppearedAction(AppBar,
                    FloatingActionButton);
            else if (fragment is ContactDetailsFragment contactDetailsFragment)
                new ContactDetailsTransitionSubViewAction(contactDetailsFragment).OnSubViewAppearedAction(AppBar,
                    FloatingActionButton);
            else if (IsFinanceListFragment(fragment))
                new FinanceListTransitionSubViewAction(_navigationIcon).OnSubViewAppearedAction(AppBar, FloatingActionButton);
            else if (IsBudgetListFragment(fragment))
                new BudgetListTransitionSubViewAction(_navigationIcon).OnSubViewAppearedAction(AppBar, FloatingActionButton);
            else if (fragment is ContactListFragment)
                new ContactListTransitionSubViewAction(_navigationIcon).OnSubViewAppearedAction(AppBar,FloatingActionButton);
            else if (fragment is StatisticsFragment)
                new StatisticsTransitionSubViewAction(_navigationIcon).OnSubViewAppearedAction(AppBar, FloatingActionButton);
            else if (fragment is BudgetDetailsFragment)
                new BudgetDetailsTransitionSubViewAction().OnSubViewAppearedAction(AppBar, FloatingActionButton);            
            else
                new DefaultTransitionSubViewAction(_navigationIcon).OnSubViewAppearedAction(AppBar, FloatingActionButton);
        }
        
        private void SetCurrentSubPage(SelectedSubPage subPage)
        {
            _viewModelProvider().CurrentlyActiveSubPage = subPage;
        }
        
        private SelectedSubPage GetSubPage(Fragment fromFragment)
        {
            if (fromFragment is FinanceDetailsFragment)
                return SelectedSubPage.FinanceDetails;
            else if (fromFragment is ContactDetailsFragment)
                return SelectedSubPage.ContactDetails;
            else if (fromFragment is ContactListFragment)
                return SelectedSubPage.Contacts;
            else if (fromFragment is AllFinancesFragment)
                return SelectedSubPage.All;
            else if (fromFragment is MyLoansFragment)
                return SelectedSubPage.Loans;
            else if (fromFragment is MyDebtsFragment)
                return SelectedSubPage.Debts;
            else if (fromFragment is FavoritesFinancesFragment)
                return SelectedSubPage.FavoritesFinances;
            else if (fromFragment is BudgetFragment)
                return SelectedSubPage.Budget;
            else if (fromFragment is StatisticsFragment)
                return SelectedSubPage.Statistics;
            else if (fromFragment is SettingsFragment)
                return SelectedSubPage.Settings;
            else if (fromFragment is AddFinanceOperationFragment)
                return SelectedSubPage.AddOperation;
            else if (fromFragment is AddBudgetItemFragment)
                return SelectedSubPage.AddBudgetItem;
            else if (fromFragment is AddContactFragment)
                return SelectedSubPage.AddContact;
            else if (fromFragment is PickContactFragment)
                return SelectedSubPage.PickContact;
            else if (fromFragment is RateAppFragment)
                return SelectedSubPage.RateApp;
            else if (fromFragment is GoPremiumFragment)
                return SelectedSubPage.GoPremium;
            else if (fromFragment is BudgetDetailsFragment)
                return SelectedSubPage.BudgetDetails;

            throw new InvalidOperationException(fromFragment.GetType().ToString());
        }

        public void OnSubViewDisappeared(FragmentManager fragmentManager, Fragment fragmentToSkip)
        {
            var lastFragment = fragmentManager.Fragments.LastOrDefault(x => x != fragmentToSkip && (x is MvxFragment || x is MvxDialogFragment || x is MvxPreferenceFragmentCompat));
            
            if (lastFragment!=null)
                OnSubViewAppeared(lastFragment);
        }
    
        bool IsFinanceListFragment(Fragment fragment)
        {
            return fragment is AllFinancesFragment || fragment is MyDebtsFragment || fragment is MyLoansFragment || 
                   fragment is FavoritesFinancesFragment;
        }

        bool IsBudgetListFragment(Fragment fragment)
        {
            return fragment is BudgetFragment;
        }
    }
}