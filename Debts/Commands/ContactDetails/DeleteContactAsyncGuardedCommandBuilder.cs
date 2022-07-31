using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Contacts;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Resources;
using Debts.Services;
using Debts.ViewModel;

namespace Debts.Commands.ContactDetails
{
    public class DeleteContactAsyncGuardedCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly ContactDetailsViewModel _detailsViewModel;

        public DeleteContactAsyncGuardedCommandBuilder(QueryCommandExecutor queryCommandExecutor, ContactDetailsViewModel detailsViewModel)
        {
            _queryCommandExecutor = queryCommandExecutor;
            _detailsViewModel = detailsViewModel;
        }

        protected override bool ShouldNotifyAboutProgress => false;

        protected override async Task ExecuteCommandAction()
        {
            Subject<bool> questionAnswerSubject = new Subject<bool>();
            
            ServicesLocation.Messenger.Publish(new QuestionMessageDialogMvxMessage(
                TextResources.Command_DeleteContact_QuestionDialog_Title,
                TextResources.Command_DeleteContact_QuestionDialog_Content, 
                this)
            {
                OnNo = () => questionAnswerSubject.OnNext(false),
                OnYes = () => questionAnswerSubject.OnNext(true)
            });
            
            bool shouldDeleteOperation = await questionAnswerSubject.FirstAsync();

            if (!shouldDeleteOperation)
                return;

            await _queryCommandExecutor.Execute(new DeleteContactDatabaseCommand(_detailsViewModel.Details));
            
            EnqueueAfterCommandExecuted(() =>
            {
                _detailsViewModel.Close.Execute();
                ServicesLocation.Messenger.Publish(new ItemRemovedMessage<Data.ContactDetails>(this, _detailsViewModel.Details));
            });
        }
    }
}