using System.Threading.Tasks;
using Debts.Data;
using SQLite;

namespace Debts.DataAccessLayer.FinanceOperations
{
    public class AddFinanceOperationNoteCommand : IDataQuery<Note>
    {
        private readonly Note _noteToAdd;

        public AddFinanceOperationNoteCommand(Note noteToAdd)
        {
            _noteToAdd = noteToAdd;
        }
        public async Task<Note> Query(SQLiteAsyncConnection connection)
        {
            await connection.InsertAsync(_noteToAdd);
            return _noteToAdd;
        }
    }
}