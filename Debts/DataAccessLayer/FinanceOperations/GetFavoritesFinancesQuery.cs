using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer.Utilities;
using SQLite;

namespace Debts.DataAccessLayer.FinanceOperations
{
    public class GetFavoritesFinancesQuery : GetAllFinanceOperationsQuery
    {
        public GetFavoritesFinancesQuery(FinanceOperationsQueryParameter limitOffsetQueryParameter) : base(limitOffsetQueryParameter)
        {
        }

        protected override QueryWhereParameter GetQueryWhere()
        {
            var queryWhere = base.GetQueryWhere();
            
            queryWhere.WhereClause.Insert(0, new WhereClause("operation.IsFavourite = 1", WhereClause.And()));

            return queryWhere;
        }
    }
}