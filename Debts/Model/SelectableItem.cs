using System.ComponentModel;
using System.Runtime.CompilerServices;
using Debts.Annotations;

namespace Debts.Model
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class SelectableItem<T> : INotifyPropertyChanged
    {
        public SelectableItem(T item)
        {
            Item = item;
        }
        
        public T Item { get; }
        public bool IsSelected { get; set; }

        public bool IsSelectionEnabled { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}