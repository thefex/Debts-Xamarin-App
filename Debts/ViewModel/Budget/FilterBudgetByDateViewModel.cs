using System;
using Debts.Commands;
using Debts.Messenging.Messages;
using Debts.Resources;
using Debts.Services;
using MvvmCross.Commands;

namespace Debts.ViewModel.Budget
{
    public class FilterBudgetByDateViewModel : BaseViewModel<string>
    {
        private readonly BudgetListViewModel _budgetListViewModel;
        public const string StartDatePickerTag = "StartDateBudget";
        public const string EndDatePickerTag = "EndDateBudget";
        

        public FilterBudgetByDateViewModel(BudgetListViewModel budgetListViewModel)
        {
            _budgetListViewModel = budgetListViewModel;
            
            StartDate = _budgetListViewModel.FilterStartDate ?? new DateTime(2020, 1, 1);
            EndDate = _budgetListViewModel.FilterEndDate;
        }
 
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public MvxCommand SelectStartDate => new MvxExceptionGuardedCommand((() =>
        {
            ServicesLocation.Messenger.Publish(new PickDateMvxMessage(this) {Tag = StartDatePickerTag});
        }));
        
        public MvxCommand SelectEndDate => new MvxExceptionGuardedCommand((() =>
        {
            ServicesLocation.Messenger.Publish(new PickDateMvxMessage(this) {Tag = EndDatePickerTag });
        }));
        
        public MvxCommand Filter => new MvxExceptionGuardedCommand(() =>
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                ServicesLocation.Messenger.Publish(new MessageDialogMvxMessage(
                    TextResources.Dialog_Error_Title, 
                    TextResources.Dialog_Error_DateFilterFinances_Content, this));
                return;
            }
                
            if (EndDate < StartDate)
            {
                ServicesLocation.Messenger.Publish(new MessageDialogMvxMessage(
                    TextResources.Dialog_Error_Title, 
                    TextResources.Dialog_Error_EndDateFilterFinances_Content, this));
                return;
            }
                
            _budgetListViewModel.FilterStartDate = StartDate;
            _budgetListViewModel.FilterEndDate = EndDate;

            _budgetListViewModel.Filter.Execute();
                
            ServicesLocation.NavigationService.Close(this);
        });
    }
}