using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Debts.Data.Annotations;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Debts.Data
{
    [Table("budget_category")]
    public class BudgetCategory : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int MainCategoryId { get; set; }
        
        public string Name { get; set; }
        
        public string AssetName { get; set; }
        
        public string ColorHex { get; set; }

        public override string ToString()
        {
            return Name.ToString();
        }
        
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<BudgetItem> Operations { get; set; }

        public void RaiseNameChanged()
        {
            OnPropertyChanged(nameof(Name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}