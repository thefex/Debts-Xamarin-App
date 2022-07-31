using System.Collections.Generic;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Data.Queries;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Debts.DataAccessLayer.Contacts
{
    public class GetAllContactsQuery : IListDataQuery<ContactDetails>
    {
        private readonly ContactListQueryParameter _limitOffsetQueryParameter;

        public GetAllContactsQuery(ContactListQueryParameter limitOffsetQueryParameter)
        {
            _limitOffsetQueryParameter = limitOffsetQueryParameter;
        }
        public async Task<IEnumerable<ContactDetails>> Query(SQLiteAsyncConnection connection)
        {
            string sql = "SELECT * " +
                         "FROM contacts contact ";

            List<object> parameters = new List<object>();
            if (!string.IsNullOrEmpty(_limitOffsetQueryParameter.SearchQuery))
            {
                sql += "WHERE ( lower(contact.FirstName) || lower(contact.LastName)) LIKE ? OR " +
                    "lower(contact.FirstName) LIKE ? OR " +
                    "lower(contact.LastName) LIKE ? ";
                parameters.Add("%" + _limitOffsetQueryParameter.SearchQuery.ToLower() + "%");
                parameters.Add("%" + _limitOffsetQueryParameter.SearchQuery.ToLower() + "%");
                parameters.Add("%" + _limitOffsetQueryParameter.SearchQuery.ToLower() + "%");
            }

            sql += "ORDER BY " +
                   "contact.FirstName ASC, " +
                   "contact.LastName ASC " +
                   "LIMIT ?,? ";
 
            parameters.Add(_limitOffsetQueryParameter.Offset);
            parameters.Add(_limitOffsetQueryParameter.Limit);

            var contacts = await connection.QueryAsync<ContactDetails>(sql, parameters.ToArray());
            
            foreach (var contact in contacts) 
                await connection.GetChildrenAsync(contact, true);

            return contacts;
        }
    }
}