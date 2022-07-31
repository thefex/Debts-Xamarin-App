using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer.Utilities;
using SQLite;

namespace Debts.DataAccessLayer.FinanceOperations
{
    public class GetMyDebtsQuery : GetAllFinanceOperationsQuery
    {
        public GetMyDebtsQuery(FinanceOperationsQueryParameter limitOffsetQueryParameter) : base(limitOffsetQueryParameter)
        {
        }

        protected override QueryWhereParameter GetQueryWhere()
        {
            var query = base.GetQueryWhere();
            
            query.WhereClause.Insert(0, new WhereClause("operation.Type = 1", WhereClause.And()));  // finance operation type enum = 1 = debt
            return query;
        }
    }
}