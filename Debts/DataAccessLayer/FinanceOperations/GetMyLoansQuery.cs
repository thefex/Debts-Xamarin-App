using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer.Utilities;
using SQLite;

namespace Debts.DataAccessLayer.FinanceOperations
{
    public class GetMyLoansQuery : GetAllFinanceOperationsQuery
    {
        public GetMyLoansQuery(FinanceOperationsQueryParameter limitOffsetQueryParameter) : base(limitOffsetQueryParameter)
        {
            
        }

        protected override QueryWhereParameter GetQueryWhere()
        {
            var query = base.GetQueryWhere();
            
            query.WhereClause.Insert(0, new WhereClause("operation.Type = 0", WhereClause.And())); // finance operation type enum = 0 = loan
            return query;
        }
    }
}