using System;
using System.Collections.Generic;

namespace Debts.Model
{
    public class BudgetItemGroup : IGroupedData 
    {
        public DateTime ForDate { get; set; }
        
        public IEnumerable<object> GroupChilds { get; set; }

        public string GetGroupUniqueKey()
        {
            return ForDate.Ticks.ToString(); 
        }
    }
}