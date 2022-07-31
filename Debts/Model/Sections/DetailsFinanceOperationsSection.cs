using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Debts.Annotations;
using Debts.Data;

namespace Debts.Model.Sections
{
    public class DetailsFinanceOperationsSection : DetailsSection, INotifyPropertyChanged
    {
        private ObservableCollection<FinanceOperation> _operations;

        public ObservableCollection<FinanceOperation> Operations
        {
            get => _operations;
            set
            {
                if (_operations != value)
                {
                    var oldOperations = _operations;

                    if (oldOperations != null)
                        oldOperations.CollectionChanged -= OperationsOnCollectionChanged;
                    
                    _operations = value;
                    
                    if (_operations!=null)
                        _operations.CollectionChanged += OperationsOnCollectionChanged;
                }
            }
        }

        private void OperationsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasAnyOperation));
            OnPropertyChanged(nameof(ItemsCount));
        }

        public bool HasAnyOperation => Operations?.Any() ?? false;

        public int ItemsCount => Operations?.Count() ?? 0;

        public override IEnumerable<object> GroupChilds => Operations;
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}