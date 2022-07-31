using System.Collections.Generic;

namespace Debts.Model
{
    public interface IGroupedData
    {
        IEnumerable<object> GroupChilds { get; }
    }
}