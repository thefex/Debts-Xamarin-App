using System;

namespace Debts.Data.Queries
{
    public class FinanceOperationsQueryParameter : LimitOffsetQueryParameter
    {
        public bool IsPaymentDeadlineExceedEnabled { get; set; }  

        public bool IsActivePaymentEnabled { get; set; }  

        public bool IsPaidOffPaymentEnabled { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public string SearchQuery { get; set; }
    }
}