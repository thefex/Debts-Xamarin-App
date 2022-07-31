using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Contacts;

namespace Debts.Services.Contacts
{
    public class ImportContactsStoreInDatabaseProxy : IPhoneContactsService 
    {
        private readonly IPhoneContactsService _proxiedService;
        private readonly QueryCommandExecutor _queryCommandExecutor;

        public ImportContactsStoreInDatabaseProxy(IPhoneContactsService proxiedService, QueryCommandExecutor queryCommandExecutor)
        {
            _proxiedService = proxiedService;
            _queryCommandExecutor = queryCommandExecutor;
        }
 
        public Task<IEnumerable<ContactDetails>> GetPhoneContacts() => _proxiedService.GetPhoneContacts();

        public async Task<IEnumerable<ContactDetails>> ImportContacts(IEnumerable<ContactDetails> contacts)
        {
            contacts = await _proxiedService.ImportContacts(contacts);

            if (contacts.Any())
                return await _queryCommandExecutor.Execute(new SaveContactsCommand(contacts));

            return contacts;
        }

        public Task<bool> CheckHasImportedContactsInPast() => _proxiedService.CheckHasImportedContactsInPast();
        public Task SetContactsAsImported() => _proxiedService.SetContactsAsImported();
    }
}