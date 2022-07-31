using System.Threading.Tasks;
using SQLite;

namespace Debts.DataAccessLayer
{
    public interface IDataCommand
    {
        Task ExecuteCommand(SQLiteAsyncConnection connection);
    }
}