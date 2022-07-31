using SQLite;

namespace Debts.Data
{
    [Table("app_schema")]
    public class AppSchemaVersionData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public int Number { get; set; }
    }
}