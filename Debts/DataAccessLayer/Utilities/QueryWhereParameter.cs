using System.Collections.Generic;

namespace Debts.DataAccessLayer.Utilities
{
    public class QueryWhereParameter
    {
        public QueryWhereParameter()
        {
        }
        public List<WhereClause> WhereClause { get; } = new List<WhereClause>();
        
        public List<object> WhereClauseArguments { get; } = new List<object>();
        
    }

    public class WhereClause
    {
        public WhereClause(string sql, string separator)
        {
            Sql = sql;
            Separator = separator;
        }
        public string Sql { get; set; }

        public string Separator { get; set; } = string.Empty;
        
        public static string And() => " AND ";

        public static string Or() => " OR ";
 

    }
}