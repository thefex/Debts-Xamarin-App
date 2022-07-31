using System;
using System.Net.Sockets;
using Debts.iOS.ViewControllers.Budget;
using Debts.iOS.ViewControllers.Contacts;
using Debts.iOS.ViewControllers.Finances;
using Debts.iOS.ViewControllers.Main;
using Debts.Model;
using Debts.ViewModel;
using Debts.ViewModel.FinancesViewModel;
using MaterialComponents;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace Debts.iOS.ViewControllers.Transition
{
    public class SubViewTransitionInvoker
    {
        private readonly Func<MainViewModel> _viewModelProvider;

        public SubViewTransitionInvoker(Func<MainViewModel> viewModelProvider)
        {
            _viewModelProvider = viewModelProvider;
        }
        
        public BottomAppBarView AppBar { get; set; }
          
        public void OnSubViewAppeared(UIViewController viewController)
        { 
            var selectedSubPage = GetSubPage(viewController);
            SetCurrentSubPage(selectedSubPage);

            if (selectedSubPage == SelectedSubPage.AddOperationNote)
                return;
               
            if (IsFinanceViewController(viewController))
                new FinanceListSubViewTransitionAction(() =>
                {
                    ((viewController as MvxViewController).ViewModel as BaseFinancesViewModel).FilterByDate.Execute();
                }, () =>
                {
                    ((viewController as MvxViewController).ViewModel as BaseFinancesViewModel).FilterByState.Execute();
                }).OnSubViewAppearedAction(AppBar);
            else if (viewController is FinanceDetailsViewController financeDetailsViewController)
                new FinanceDetailsSubViewTransitionAction(() =>
                {
                    var vc = financeDetailsViewController;

                    return financeDetailsViewController.ViewModel;
                }).OnSubViewAppearedAction(AppBar);
            else if (viewController is BudgetViewController budgetViewController)
                new BudgetListSubViewTransitionAction(() => budgetViewController.ViewModel).OnSubViewAppearedAction(AppBar);
            else if (viewController is ContactsViewController contactsViewController)
                new ContactListSubViewTransitionAction(() => contactsViewController.ViewModel).OnSubViewAppearedAction(AppBar);
            else if (viewController is StatisticsViewController statisticsViewController)
                new StatisticsSubViewTransitionAction(() => statisticsViewController.ViewModel).OnSubViewAppearedAction(AppBar);
            else if (viewController is BudgetDetailsViewController)
                new BudgetDetailsSubviewTransitionAction().OnSubViewAppearedAction(AppBar);
            else
                new DefaultSubviewTransitionAction().OnSubViewAppearedAction(AppBar);
        }

        bool IsFinanceViewController(UIViewController viewController)
        {
            return viewController is AllFinancesViewController ||
                   viewController is DebtsFinancesViewController ||
                   viewController is LoansFinancesViewController ||
                   viewController is FavoritesFinancesViewController;
        }
        
        private void SetCurrentSubPage(SelectedSubPage subPage)
        {
            _viewModelProvider().CurrentlyActiveSubPage = subPage;
        }
        
        private SelectedSubPage GetSubPage(UIViewController fromFragment)
        { 
            if (fromFragment is ContactDetailsViewController)
                return SelectedSubPage.ContactDetails;
            if (fromFragment is ContactsViewController)
                return SelectedSubPage.Contacts;
            else if (fromFragment is AllFinancesViewController)
                return SelectedSubPage.All;
            else if (fromFragment is LoansFinancesViewController)
                return SelectedSubPage.Loans;
            else if (fromFragment is DebtsFinancesViewController)
                return SelectedSubPage.Debts;
            else if (fromFragment is FavoritesFinancesViewController)
                return SelectedSubPage.FavoritesFinances;
            else if (fromFragment is BudgetViewController)
                return SelectedSubPage.Budget;
            else if (fromFragment is StatisticsViewController)
                return SelectedSubPage.Statistics;
            else if (fromFragment is SettingsViewController)
                return SelectedSubPage.Settings;
            else if (fromFragment is AddFinanceViewController)
                return SelectedSubPage.AddOperation;
            else if (fromFragment is AddBudgetViewController)
                return SelectedSubPage.AddBudgetItem;
            else if (fromFragment is AddContactViewController)
                return SelectedSubPage.AddContact;
            else if (fromFragment is AddFinanceNoteViewController)
                return SelectedSubPage.AddOperationNote;
            else if (fromFragment is ImportContactsViewController)
                return SelectedSubPage.ImportContacts;
            else if (fromFragment is PickContactViewController)
                return SelectedSubPage.PickContact; 
            else if (fromFragment is FinanceDetailsViewController)
                return SelectedSubPage.FinanceDetails;
            else if (fromFragment is RateAppViewController)
                return SelectedSubPage.RateApp;
            else if (fromFragment is GoPremiumViewController)
                return SelectedSubPage.GoPremium;
            else if (fromFragment is PickBudgetCategoryViewController)
                return SelectedSubPage.PickBudgetCategory;
            else if (fromFragment is BudgetDetailsViewController)
                return SelectedSubPage.BudgetDetails;
            
            throw new InvalidOperationException(fromFragment.GetType().ToString());
        }
    }
}