using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Budget;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Resources;
using Debts.Services;
using Debts.ViewModel.Budget;

namespace Debts.Commands.Budget
{
    public class DeleteBudgetCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly BudgetDetailsViewModel _detailsViewModel;

        public DeleteBudgetCommandBuilder(QueryCommandExecutor queryCommandExecutor, BudgetDetailsViewModel detailsViewModel)
        {
            _queryCommandExecutor = queryCommandExecutor;
            _detailsViewModel = detailsViewModel;
        }

        protected override bool ShouldNotifyAboutProgress => false;

        protected override async Task ExecuteCommandAction()
        {
            Subject<bool> questionAnswerSubject = new Subject<bool>();
            
            ServicesLocation.Messenger.Publish(new QuestionMessageDialogMvxMessage(TextResources.Command_DeleteBudgetOperation_QuestionDialog_Title,
                TextResources.Command_DeleteBudget_QuestionDialog_Content,
                this)
            {
                OnNo = () => questionAnswerSubject.OnNext(false),
                OnYes = () => questionAnswerSubject.OnNext(true)
            });
            
            bool shouldDeleteOperation = await questionAnswerSubject.FirstAsync();

            if (!shouldDeleteOperation)
                return;

            await _queryCommandExecutor.Execute(new DeleteBudgetItemCommand(_detailsViewModel.Item));
            
            EnqueueAfterCommandExecuted(() =>
            {
                _detailsViewModel.Close.Execute();
                ServicesLocation.Messenger.Publish(new ItemRemovedMessage<BudgetItem>(this, _detailsViewModel.Item));
            });
        }
    }
}