using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Debts.Data;
using SQLite;

namespace Debts.DataAccessLayer.Contacts
{
    public class SaveContactsCommand : IDataQuery<IEnumerable<ContactDetails>>
    {
        private readonly IEnumerable<ContactDetails> _contactsToSave;

        public SaveContactsCommand(IEnumerable<ContactDetails> contactsToSave)
        {
            _contactsToSave = contactsToSave;
        }
 
        public async Task<IEnumerable<ContactDetails>> Query(SQLiteAsyncConnection connection)
        {
            var deviceIds = _contactsToSave.Where(contact => contact.DeviceBasedId.HasValue)
                .Select(contact => contact.DeviceBasedId)
                .ToList();

            IEnumerable<ContactDetails> contactsToDoNotSave = Enumerable.Empty<ContactDetails>();
            

            if (deviceIds.Any())
            {
                contactsToDoNotSave = await connection.Table<ContactDetails>()
                    .Where(x => deviceIds.Contains(x.DeviceBasedId))
                    .ToListAsync();
            }
      
            List<ContactDetails> savedContacts = new List<ContactDetails>();
            foreach (var contactDetails in _contactsToSave)
            {
                if (contactDetails.DeviceBasedId.HasValue &&
                    contactsToDoNotSave.Any(x => x.DeviceBasedId.Equals(contactDetails.DeviceBasedId)))
                    continue;

                await connection.InsertAsync(contactDetails);
                savedContacts.Add(contactDetails);
            }

            return savedContacts;
        }
    }
}