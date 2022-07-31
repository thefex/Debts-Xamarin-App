using System;
using Debts.Commands;
using Debts.Messenging.Messages;
using Debts.Resources;
using Debts.Services;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Debts.ViewModel.Finances
{
    public class FilterFinancesByDateViewModel : BaseViewModel<string>
    {
        public const string StartDatePickerTag = "StartDate";
        public const string EndDatePickerTag = "EndDate";
        
        private readonly BaseFinancesViewModel _financesViewModel;

        public FilterFinancesByDateViewModel(BaseFinancesViewModel financesViewModel)
        {
            _financesViewModel = financesViewModel;
            

            StartDate = _financesViewModel.FilterStartDate ?? new DateTime(2020, 1, 1);;
            EndDate = _financesViewModel.FilterEndDate;
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
                
                _financesViewModel.FilterStartDate = StartDate;
                _financesViewModel.FilterEndDate = EndDate;

                _financesViewModel.Filter.Execute();
                ServicesLocation.NavigationService.Close(this);
            });
    }
}