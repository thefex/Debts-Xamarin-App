using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Contacts;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Resources;
using Debts.Services;
using Debts.ViewModel.Contacts;
using MvvmCross;

namespace Debts.Commands.Contacts
{
    public class AddContactCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly AddContactViewModel _addContactViewModel;
        private readonly QueryCommandExecutor _queryCommandExecutor;

        public AddContactCommandBuilder(AddContactViewModel addContactViewModel, QueryCommandExecutor queryCommandExecutor)
        {
            _addContactViewModel = addContactViewModel;
            _queryCommandExecutor = queryCommandExecutor;
        }

        protected override async Task ExecuteCommandAction()
        {
            if (string.IsNullOrEmpty(_addContactViewModel.ContactName))
            {
                ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, TextResources.Command_AddContact_ToastErrorContent)
                {
                    Style = ToastMvxMessage.ToastStyle.Error
                });
                return;
            }
            
            var insertedContact = await _queryCommandExecutor.Execute(new AddNewContactCommandQuery(
                _addContactViewModel.ContactName,
                _addContactViewModel.PhoneNumber,
                _addContactViewModel.PhotoPath));
            
            EnqueueAfterCommandExecuted(() =>
            {
                _addContactViewModel.Close.Execute();
                ServicesLocation.Messenger.Publish(new ItemPublishedMessage<Data.ContactDetails>(this, insertedContact));
            });
        }
    }
}