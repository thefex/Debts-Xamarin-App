using System.Collections.Generic;
using System.Linq;

namespace Debts.Model.Sections
{
    public class DetailsSection : IGroupedData
    {
        public string Title { get; set; }
        
        public string ImageName { get; set; }
        
        public virtual IEnumerable<object> GroupChilds => Enumerable.Empty<object>();
    }
}