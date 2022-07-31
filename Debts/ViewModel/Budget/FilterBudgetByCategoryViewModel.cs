using Debts.Commands;
using Debts.Data;
using Debts.Messenging;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Resources;
using Debts.Services;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross.Commands;

namespace Debts.ViewModel.Budget
{
    public class FilterBudgetByCategoryViewModel : BaseViewModel<string>
    {
        private readonly BudgetListViewModel _budgetListViewModel;

        public FilterBudgetByCategoryViewModel(BudgetListViewModel budgetListViewModel)
        {
            _budgetListViewModel = budgetListViewModel;
            SelectedFilterCategory = budgetListViewModel.SelectedFilterCategory;

            MessageObserversController.AddObservers(new InvokeActionMessageObserver<BudgetCategorySelectedMvxMessage>(
                msg => { SelectedFilterCategory = msg.BudgetCategory; }));
        } 

        public BudgetCategory SelectedFilterCategory { get; set; }  
 
        public MvxCommand SelectCategory => new MvxExceptionGuardedCommand(() =>
            {
                ServicesLocation.NavigationService.Navigate<PickBudgetCategoryForFilterViewModel>();
            });
        
        public override void ViewAppearing()
        {
            base.ViewAppearing();
            ServicesLocation.MessageQueue.ResendMessages<BudgetCategorySelectedMvxMessage>();
        }
        
        public MvxCommand Filter => new MvxExceptionGuardedCommand(() =>
        { 
            if (SelectedFilterCategory == null)
            {
                ServicesLocation.Messenger.Publish(new MessageDialogMvxMessage(
                    TextResources.Dialog_Error_Title,
                    TextResources.Dialog_Error_FilterCategory_Content, this));
                return;
            }

            _budgetListViewModel.SelectedFilterCategory = SelectedFilterCategory;
            _budgetListViewModel.Filter.Execute();
            
            ServicesLocation.NavigationService.Close(this);
        });
    }
}