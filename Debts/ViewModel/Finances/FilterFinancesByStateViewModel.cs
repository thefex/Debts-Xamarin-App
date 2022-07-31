using Debts.Commands;
using Debts.Messenging.Messages;
using Debts.Resources;
using Debts.Services;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Debts.ViewModel.Finances
{
    public class FilterFinancesByStateViewModel : BaseViewModel<string>
    {
        private readonly BaseFinancesViewModel _financesViewModel;

        public FilterFinancesByStateViewModel(BaseFinancesViewModel financesViewModel)
        {
            _financesViewModel = financesViewModel;
            
            IsPaidOffPaymentEnabled = _financesViewModel.IsPaidOffPaymentEnabled;
            IsActivePaymentEnabled = _financesViewModel.IsActivePaymentEnabled;
            IsPaymentDeadlineExceedEnabled = _financesViewModel.IsPaymentDeadlineExceedEnabled;
        } 

        public bool IsPaymentDeadlineExceedEnabled { get; set; }  

        public bool IsActivePaymentEnabled { get; set; }  
        
        public bool IsPaidOffPaymentEnabled { get; set; }
        
        public MvxCommand Filter => new MvxExceptionGuardedCommand(() =>
        { 
            if (!IsActivePaymentEnabled && 
                !IsPaidOffPaymentEnabled &&
                !IsPaymentDeadlineExceedEnabled)
            {
                ServicesLocation.Messenger.Publish(new MessageDialogMvxMessage(
                    TextResources.Dialog_Error_Title,
                    TextResources.Dialog_Error_AtLeastOnFilter_Content, this));
                return;
            }
            
            _financesViewModel.IsActivePaymentEnabled = IsActivePaymentEnabled;
            _financesViewModel.IsPaidOffPaymentEnabled = IsPaidOffPaymentEnabled;
            _financesViewModel.IsPaymentDeadlineExceedEnabled = IsPaymentDeadlineExceedEnabled;
            
            _financesViewModel.Filter.Execute();
            
            ServicesLocation.NavigationService.Close(this);
        });
    }
}