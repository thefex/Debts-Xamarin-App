using System.Threading.Tasks;
using Debts.Data;
using SQLite;

namespace Debts.DataAccessLayer.Contacts
{
    public class AddNewContactCommandQuery : IDataQuery<ContactDetails>
    {
        private readonly string _contactName;
        private readonly string _phoneNumber;
        private readonly string _avatarPath;

        public AddNewContactCommandQuery(string contactName, string phoneNumber = null, string avatarPath = null)
        {
            _contactName = contactName;
            _phoneNumber = phoneNumber;
            _avatarPath = avatarPath;
        }
        
        public async Task<ContactDetails> Query(SQLiteAsyncConnection connection)
        {
            var insertedContact = new ContactDetails()
            {
                FirstName = _contactName,
                PhoneNumber = _phoneNumber,
                AvatarUrl = _avatarPath
            };
            await connection.InsertAsync(insertedContact);
            return insertedContact;
        }
    }
}