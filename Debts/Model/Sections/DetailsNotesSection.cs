using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Debts.Annotations;
using Debts.Data;
using MvvmCross.ViewModels;

namespace Debts.Model.Sections
{
    public class DetailsNotesSection : DetailsSection, INotifyPropertyChanged
    {
        public DetailsNotesSection()
        {
                
        }

        private ObservableCollection<Note> _notes;
        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set
            {
                if (_notes != value)
                {
                    var oldNotes = _notes;
                    _notes = value;

                    if (oldNotes != null)
                        oldNotes.CollectionChanged -= NotesOnCollectionChanged;
                    
                    if (_notes!=null)
                        _notes.CollectionChanged += NotesOnCollectionChanged;
                }
            }
        }

        private void NotesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasAnyNote));
            OnPropertyChanged(nameof(ItemsCount));
        }

        public bool HasAnyNote => Notes?.Any() ?? false;

        public int ItemsCount => Notes?.Count() ?? 0;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override IEnumerable<object> GroupChilds => Notes;
    }
}