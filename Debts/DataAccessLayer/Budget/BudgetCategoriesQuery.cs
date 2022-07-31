using System.Collections.Generic;
using System.Threading.Tasks;
using Debts.Data;
using SQLite;

namespace Debts.DataAccessLayer.Budget
{
    public class BudgetCategoriesQuery : IDataQuery<IEnumerable<BudgetCategory>>
    {
        public async Task<IEnumerable<BudgetCategory>> Query(SQLiteAsyncConnection connection)
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                return await connection.Table<BudgetCategory>()
                    .OrderBy(x => x.Name)
                    .ToListAsync();
            }

            string sql =
                @"SELECT * FROM budget_category category WHERE lower(category.Name) LIKE ? ORDER BY category.Name DESC";
            return await connection.QueryAsync<BudgetCategory>(sql, "%" + SearchQuery.ToLower() + "%");
        }

        public string SearchQuery { get; set; }
    }
}