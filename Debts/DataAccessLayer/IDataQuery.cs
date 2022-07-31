using System.Threading.Tasks;
using SQLite;

namespace Debts.DataAccessLayer
{
    public interface IDataQuery<TResult>
    {
        Task<TResult> Query(SQLiteAsyncConnection connection);
    }
}