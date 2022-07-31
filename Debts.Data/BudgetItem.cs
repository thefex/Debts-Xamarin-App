using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Debts.Data.Annotations;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Debts.Data
{
    [Table("budget")]
    public class BudgetItem : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        [ForeignKey(typeof(BudgetCategory))]
        public int CategoryId { get; set; }
        
        [ManyToOne]
        public BudgetCategory Category { get; set; }
        
        public string Currency { get; set; }
        
        public decimal Amount { get; set; }
        
        public string Title { get; set; }
        
        public string FormattedAmount
        {
            get
            { 
                return $"{Amount} ({Currency})";
            }
        }

        public string ShortFormattedAmount
        {
            get
            {
                if (Amount > 999)
                    return "$999+";

                return "$" + Amount;
            } 
        }
        
        public BudgetType Type { get; set; }

        [Ignore] public bool IsExpense => Type == BudgetType.Expense;

        [Ignore] public bool IsIncome => Type == BudgetType.Income;
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaiseTitleAmountChanged()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Amount));
        }
 
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}