using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Messenging.Messages.App;
using Debts.Services;
using Debts.Services.Contacts;
using Debts.ViewModel.Contacts;

namespace Debts.Commands.Contacts
{
    public class ImportContactsCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly ImportContactsViewModel _importContactsViewModel;
        private readonly IPhoneContactsService _phoneContactsService;

        public ImportContactsCommandBuilder(ImportContactsViewModel importContactsViewModel, IPhoneContactsService phoneContactsService)
        {
            _importContactsViewModel = importContactsViewModel;
            _phoneContactsService = phoneContactsService;
        }
        
        protected override async Task ExecuteCommandAction()
        {
            var importedContacts = await _phoneContactsService.ImportContacts(_importContactsViewModel.ContactsToImport);
            EnqueueAfterCommandExecuted(() =>
            {
                ServicesLocation.Messenger.Publish(new ItemPublishedMessage<IEnumerable<Data.ContactDetails>>(this, importedContacts));
                _importContactsViewModel.Close.Execute();
            });
        }
    }
}