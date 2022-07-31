using System.Collections.Generic;

namespace Debts.DataAccessLayer
{
    public interface IListDataQuery<TResult> : IDataQuery<IEnumerable<TResult>>
    {
        
    }
}