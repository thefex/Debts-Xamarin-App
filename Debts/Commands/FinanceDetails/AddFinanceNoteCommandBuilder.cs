using System;
using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Resources;
using Debts.Services;
using Debts.ViewModel.FinancesDetails;

namespace Debts.Commands.FinanceDetails
{
    public class AddFinanceNoteCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly AddFinanceDetailsNoteViewModel _viewModel;

        public AddFinanceNoteCommandBuilder(QueryCommandExecutor queryCommandExecutor, AddFinanceDetailsNoteViewModel viewModel)
        {
            _queryCommandExecutor = queryCommandExecutor;
            _viewModel = viewModel;
        }

        protected override bool ShouldNotifyAboutProgress => false;

        protected override async Task ExecuteCommandAction()
        {
            if (string.IsNullOrEmpty(_viewModel.Note))
            {
                ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, TextResources.Command_AddNoteEmptyText_ToastErrorContent)
                {
                    Style = ToastMvxMessage.ToastStyle.Error
                });
                return;
            }
            
            var addedNote = await _queryCommandExecutor.Execute(new AddFinanceOperationNoteCommand(new Note()
            {
                AssignedToFinanceOperationId = _viewModel.Operation.FinancePrimaryId,
                Type = _viewModel.Type,
                CreatedAt = DateTime.UtcNow,
                Text = _viewModel.Note,
                Duration = _viewModel.Duration
            }));
            
            ServicesLocation.Messenger.Publish(new ItemPublishedMessage<Note>(this, addedNote));
        }
    }
}