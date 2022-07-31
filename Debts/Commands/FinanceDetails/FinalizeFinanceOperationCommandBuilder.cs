using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Resources;
using Debts.Services;
using Debts.ViewModel;

namespace Debts.Commands.FinanceDetails
{
    public class FinalizeFinanceOperationCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly FinanceDetailsViewModel _financeDetailsViewModel;
        private readonly QueryCommandExecutor _queryCommandExecutor;

        public FinalizeFinanceOperationCommandBuilder(FinanceDetailsViewModel financeDetailsViewModel, QueryCommandExecutor queryCommandExecutor)
        {
            _financeDetailsViewModel = financeDetailsViewModel;
            _queryCommandExecutor = queryCommandExecutor;
        }
        
        protected override async Task ExecuteCommandAction()
        {
            Subject<bool> questionAnswerSubject = new Subject<bool>();
            
            ServicesLocation.Messenger.Publish(new QuestionMessageDialogMvxMessage(TextResources.Command_FinalizeFinanceOperation_QuestionDialog_Title,
                TextResources.Command_FinalizeFinanceOperation_QuestionDialog_Content,
                this)
            {
                OnNo = () => questionAnswerSubject.OnNext(false),
                OnYes = () => questionAnswerSubject.OnNext(true)
            });
            
            bool shouldFinalizeOperation = await questionAnswerSubject.FirstAsync();

            if (!shouldFinalizeOperation)
                return;

            var updatedPaymentDetails = await _queryCommandExecutor.Execute(new FinalizeFinanceOperationDatabaseCommand (_financeDetailsViewModel.Details));
            
            EnqueueAfterCommandExecuted(() =>
            {
                _financeDetailsViewModel.Details.PaymentDetails.PaymentDate = updatedPaymentDetails.PaymentDate;
                _financeDetailsViewModel.Details.OnPaymentStateChanged();
                _financeDetailsViewModel.RaisePropertyChanged(() => _financeDetailsViewModel.IsPaid);
                
                ServicesLocation.Messenger.Publish(new ItemUpdatedMessage<FinanceOperation>(this, _financeDetailsViewModel.Details));
            });
        }
    }
}