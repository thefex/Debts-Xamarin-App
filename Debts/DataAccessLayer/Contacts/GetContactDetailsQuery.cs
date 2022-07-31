using System.Threading.Tasks;
using Debts.Data;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Debts.DataAccessLayer.Contacts
{
    public class GetContactDetailsQuery : IDataQuery<ContactDetails>
    {
        private readonly ContactDetails _contactDetails;

        public GetContactDetailsQuery(ContactDetails contactDetails)
        {
            _contactDetails = contactDetails;
        }
        
        public async Task<ContactDetails> Query(SQLiteAsyncConnection connection)
        {
            await connection.GetChildrenAsync(_contactDetails, true);
            return _contactDetails;
        }
    }
}