using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Services;
using Debts.Services.Contacts;

namespace Debts.iOS.Services
{
    public class PhoneContactsService : IPhoneContactsService
    {
        private readonly IStorageService _storageService;
        private const string HasImportedContactsInPastKey = "HasImportedContactsInPastKey";

        public PhoneContactsService(IStorageService storageService)
        {
            _storageService = storageService;
        }
        
        public async Task<IEnumerable<ContactDetails>> GetPhoneContacts()
        {
            return Enumerable.Empty<ContactDetails>();
        }

        public Task<IEnumerable<ContactDetails>> ImportContacts(IEnumerable<ContactDetails> contacts)
        {
            _storageService.Store(HasImportedContactsInPastKey, true);
            return Task.FromResult(contacts);
        }

        public Task<bool> CheckHasImportedContactsInPast()
        {
            return Task.FromResult(_storageService.Get<bool>(HasImportedContactsInPastKey));
        }

        public Task SetContactsAsImported()
        {
            _storageService.Store(HasImportedContactsInPastKey, true);
            return Task.FromResult(true);
        }
    }
}