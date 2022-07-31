using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer.Utilities;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Debts.DataAccessLayer.Budget
{
    public class BudgetGetAllQuery : IListDataQuery<BudgetItem>
    {
        private readonly BudgetItemsQueryParameter _queryParameter;

        public BudgetGetAllQuery(BudgetItemsQueryParameter itemsQueryParameter)
        {
            _queryParameter = itemsQueryParameter;
        }
         
          protected virtual QueryWhereParameter GetQueryWhere()
        {
            QueryWhereParameter queryWhereParameter = new QueryWhereParameter();
            List<WhereClause> whereClause = queryWhereParameter.WhereClause;
            List<object> parameterList = queryWhereParameter.WhereClauseArguments;
             
            if (!string.IsNullOrEmpty(_queryParameter.SearchQuery))
            {
                whereClause.Add(new WhereClause("(lower(operation.Title) LIKE ? OR lower(category.Name) LIKE ? OR cast(operation.Amount as text) LIKE ?)", WhereClause.And()));
                for (int i = 0; i < 3; ++i)
                    parameterList.Add("%" + _queryParameter.SearchQuery.ToLower() + "%"); 
            }

            if (_queryParameter.StartDate.HasValue && _queryParameter.EndDate.HasValue)
            {
                whereClause.Add(new WhereClause("( " +
                                                "(operation.CreatedAt > ? AND operation.CreatedAt < ?) " +
                                                ")", WhereClause.And()));
                
                parameterList.Add(_queryParameter.StartDate.Value);
                parameterList.Add(_queryParameter.EndDate.Value);
            }
            if (!string.IsNullOrEmpty(_queryParameter.CategoryId))
            {
                string sql = "(operation.CategoryId = ?)"; 

                parameterList.Add(_queryParameter.CategoryId);
                whereClause.Add(new WhereClause(sql, WhereClause.Or()));
            }
            
            return queryWhereParameter;
        }
         
        public async Task<IEnumerable<BudgetItem>> Query(SQLiteAsyncConnection connection)
        { 
            var queryWhereParameter = GetQueryWhere();

            var sqlQuery = BuildSqlQuery(queryWhereParameter);

            var arguments = queryWhereParameter.WhereClauseArguments.Concat(new object[] {_queryParameter.Offset, _queryParameter.Limit}).ToList();

            var results =
                await connection.QueryAsync<BudgetItem>(sqlQuery, arguments.ToArray());
 
            foreach (var operation in results)
                await connection.GetChildrenAsync<BudgetItem>(operation);

            return results; 
        }

        private string BuildSqlQuery(QueryWhereParameter queryWhereParameter)
        {
            string sqlQuery = "SELECT * " +
                              "FROM budget AS operation " +
                              "JOIN budget_category AS category ON operation.CategoryId = category.MainCategoryId ";

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