using System.Threading.Tasks;
using Debts.Data;
using SQLite;

namespace Debts.DataAccessLayer.Contacts
{
    public class DeleteContactDatabaseCommand : IDataCommand
    {
        private readonly ContactDetails _contactDetails;

        public DeleteContactDatabaseCommand(ContactDetails contactDetails)
        {
            _contactDetails = contactDetails;
        }
        public Task ExecuteCommand(SQLiteAsyncConnection connection) => connection.Table<ContactDetails>().DeleteAsync(x => x.ContactPrimaryId == _contactDetails.ContactPrimaryId);
    }
}