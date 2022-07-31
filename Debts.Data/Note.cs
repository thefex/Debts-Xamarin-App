using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Debts.Data
{
    [Table("note")]
    public class Note
    { 
        public NoteType Type { get; set; }
        
        public string Text { get; set; }
        
        public DateTime CreatedAt { get; set; }

        [ForeignKey(typeof(FinanceOperation))]
        public long AssignedToFinanceOperationId { get; set; }
          
        public TimeSpan? Duration { get; set; }

        [PrimaryKey, AutoIncrement]
        public int NoteId { get; set; }
    }
}