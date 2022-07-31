using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer.Utilities;
using MvvmCross.Core;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Debts.DataAccessLayer.FinanceOperations
{
    public class GetAllFinanceOperationsQuery : IListDataQuery<FinanceOperation>
    {
        private readonly FinanceOperationsQueryParameter queryParameter;
        
        public GetAllFinanceOperationsQuery(FinanceOperationsQueryParameter limitOffsetQueryParameter)
        {
            queryParameter = limitOffsetQueryParameter;
        }

        protected virtual QueryWhereParameter GetQueryWhere()
        {
            QueryWhereParameter queryWhereParameter = new QueryWhereParameter();
            List<WhereClause> whereClause = queryWhereParameter.WhereClause;
            List<object> parameterList = queryWhereParameter.WhereClauseArguments;
             
            if (!string.IsNullOrEmpty(queryParameter.SearchQuery))
            {
                whereClause.Add(new WhereClause("(lower(operation.Title) LIKE ? OR lower(contact.FirstName) LIKE ? OR lower(contact.LastName) LIKE ? OR cast(payment.Amount as text) LIKE ?)", WhereClause.And()));
                for (int i = 0; i < 4; ++i)
                    parameterList.Add("%" + queryParameter.SearchQuery.ToLower() + "%"); 
            }

            if (queryParameter.StartDate.HasValue && queryParameter.EndDate.HasValue)
            {
                whereClause.Add(new WhereClause("( " +
                                                "(payment.PaymentDate IS NOT NULL AND payment.PaymentDate > ? AND payment.PaymentDate < ?) " +
                                                " OR " +
                                                "(payment.DeadlineDate > ? AND payment.DeadlineDate < ?)" +
                                                ")", WhereClause.And()));

                for (int i = 0; i < 2; ++i)
                {
                    parameterList.Add(queryParameter.StartDate.Value);
                    parameterList.Add(queryParameter.EndDate.Value);
                }
            }

            bool shouldOpen = queryParameter.IsPaidOffPaymentEnabled || 
                              queryParameter.IsActivePaymentEnabled ||
                              queryParameter.IsPaymentDeadlineExceedEnabled;

            string startSql = "(";
            
            if (queryParameter.IsPaidOffPaymentEnabled)
            {
                string sql = startSql +  "payment.PaymentDate IS NOT NULL";
                startSql = string.Empty;

                if (!queryParameter.IsActivePaymentEnabled && !queryParameter.IsPaymentDeadlineExceedEnabled)
                    sql += ")";
                    
                whereClause.Add(new WhereClause(sql, WhereClause.Or()));
            }

            if (queryParameter.IsActivePaymentEnabled)
            {
                string sql = startSql + "(payment.DeadlineDate > ? AND payment.PaymentDate IS NULL)";
                startSql = string.Empty;

                if (!queryParameter.IsPaymentDeadlineExceedEnabled)
                    sql += ")";

                whereClause.Add(new WhereClause(sql, WhereClause.Or()));
                parameterList.Add(DateTime.UtcNow); 
            }

            if (queryParameter.IsPaymentDeadlineExceedEnabled)
            {
                string sql = startSql +  "(payment.DeadlineDate <= ? AND payment.PaymentDate IS NULL)";
                startSql = String.Empty;
                sql += ")";
                
                whereClause.Add( new WhereClause(sql, WhereClause.Or()));
                parameterList.Add(DateTime.UtcNow);
            }
             
			return queryWhereParameter;
        }
         
        public async Task<IEnumerable<FinanceOperation>> Query(SQLiteAsyncConnection connection)
        { 
            var queryWhereParameter = GetQueryWhere();

            var sqlQuery = BuildSqlQuery(queryWhereParameter);

            var arguments = queryWhereParameter.WhereClauseArguments.Concat(new object[] {queryParameter.Offset, queryParameter.Limit}).ToList();

            var results =
                await connection.QueryAsync<FinanceOperation>(sqlQuery, arguments.ToArray());
 
            foreach (var operation in results)
                await connection.GetChildrenAsync<FinanceOperation>(operation);

            return results; 
        }

        private string BuildSqlQuery(QueryWhereParameter queryWhereParameter)
        {
            string sqlQuery = "SELECT * " +
                              "FROM finance_operations AS operation " +
                              "JOIN payments AS payment ON operation.FinancePrimaryId = payment.AssignedToFinanceOperationId " +
                              "JOIN contacts AS contact ON operation.AssignedContactId = contact.ContactPrimaryId ";

            if (queryWhereParameter.WhereClause.Any())
            {
                sqlQuery += "WHERE ";

                sqlQuery += queryWhereParameter.WhereClause[0].Sql;
                if (queryWhereParameter.WhereClause.Count > 1)
                    sqlQuery += queryWhereParameter.WhereClause[0].Separator;

                for (int i = 1; i < queryWhereParameter.WhereClause.Count; ++i)
                {
                    sqlQuery += queryWhereParameter.WhereClause[i].Sql;
                    
                    if (i != queryWhereParameter.WhereClause.Count - 1)
                        sqlQuery += queryWhereParameter.WhereClause[i].Separator;
                }
                
                sqlQuery += " ";
            }

            sqlQuery += "LIMIT ?,?";
            return sqlQuery;
        }
    }
}