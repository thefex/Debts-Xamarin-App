using System;

namespace Debts.Data.Queries
{
    public class BudgetItemsQueryParameter : LimitOffsetQueryParameter
    {
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }

        public string CategoryId { get; set; }
        
        public string SearchQuery { get; set; }
    }
}