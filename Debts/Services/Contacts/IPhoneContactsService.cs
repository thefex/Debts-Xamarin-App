using System.Collections.Generic;
using System.Threading.Tasks;
using Debts.Data;

namespace Debts.Services.Contacts
{ 
    public interface IPhoneContactsService
    {
        Task<IEnumerable<ContactDetails>> GetPhoneContacts();
        
        Task<IEnumerable<ContactDetails>> ImportContacts(IEnumerable<ContactDetails> contacts);
        
        Task<bool> CheckHasImportedContactsInPast();

        Task SetContactsAsImported();
    }
}