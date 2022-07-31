using System;
using System.Collections.Generic;
using System.Linq;

namespace Debts.Model
{
    public class FinancesOperationGroup : IGroupedData
    { 
        public DateTime ForDate { get; set; }
        
        public FinanceGroupType GroupType { get; set; }
        
        public IEnumerable<object> GroupChilds { get; set; }

        public string GetGroupUniqueKey()
        {
            if (GroupType == FinanceGroupType.Active)
                return ForDate.Ticks.ToString();

            return GroupType.ToString();
        }
    }
    public enum FinanceGroupType
    {
        DeadlinePassed = 0,
        Active = 1,
        Paid = 2
    }
}