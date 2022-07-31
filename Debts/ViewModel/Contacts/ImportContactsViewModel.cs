using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Commands.Contacts;
using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Contacts;
using Debts.Messenging;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Model;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Contacts;
using DynamicData;
using Humanizer;
using MvvmCross.Commands;

namespace Debts.ViewModel.Contacts
{
    public class ImportContactsViewModel : ListViewModel<IEnumerable<ContactDetails>, SelectableItem<ContactDetails>, ContactsGroup>
    {
        private readonly IPhoneContactsService _phoneContactsService;
        private readonly PremiumService _premiumService;
        readonly Subject<Func<SelectableItem<ContactDetails>, bool>> _searchFilterPredicateSubject = new Subject<Func<SelectableItem<ContactDetails>, bool>>();

        public ImportContactsViewModel(IPhoneContactsService phoneContactsService, PremiumService premiumService)
        {
            _phoneContactsService = phoneContactsService;
            _premiumService = premiumService;
        }

        public override void Prepare(IEnumerable<ContactDetails> parameter)
        {
            base.Prepare(parameter);
            ItemsSource.AddRange(parameter.Select(x => new SelectableItem<ContactDetails>(x){ IsSelectionEnabled = true}));
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            ServicesLocation.MessageQueue.ResendAndClearMessages<ItemPublishedMessage<IEnumerable<ContactDetails>>>();
        }

        protected override bool BuildAndSetObservable()
        {
            var result = base.BuildAndSetObservable();
            OnSearchQueryChanged();
            return result;
        }

        private void OnSearchQueryChanged() =>
            _searchFilterPredicateSubject.OnNext(x =>
                string.IsNullOrEmpty(RecentSearchWord) ||
                x.Item.ToString().ToLower().Contains(RecentSearchWord.Trim().ToLower()));
         
        protected override ISourceList<SelectableItem<ContactDetails>> ItemsSource { get; } = new SourceList<SelectableItem<ContactDetails>>();
        protected override IObservable<IChangeSet<ContactsGroup>> GetObservableChangeSet() => ItemsSource
            .Connect()
            .Filter(_searchFilterPredicateSubject)
            .Sort(Comparer<SelectableItem<ContactDetails>>.Create((x, y) => String.Compare(x.Item.ToString(), y.Item.ToString(), StringComparison.Ordinal)))
            .OnItemAdded(x => x.PropertyChanged += ContactPropertyChanged)
            .OnItemRemoved(x => x.PropertyChanged -= ContactPropertyChanged)
            .GroupOn(x =>
            {
                var firstCharacter = (x.Item.FullName.Trim().FirstOrDefault());
                if (firstCharacter == '\0')
                    firstCharacter = ' ';

                return firstCharacter.ToString().ToUpper();
            })
            .Transform(CreateContactsGroup);

        public bool HasAnyContactsToImport => ContactsToImport.Any();
        
        private ContactsGroup CreateContactsGroup(IGroup<SelectableItem<ContactDetails>, string> group)
        {
            var contactsGroup = new ContactsGroup()
            {
                GroupingLetter = group.GroupKey.ToUpper()
            };
            
            ReadOnlyObservableCollection<SelectableItem<ContactDetails>> childItemsCollection;

            var groupChangeSet = group.List.Connect();
  
            var subscriptions = groupChangeSet
                .ObserveOn(SynchronizationContext.Current)
                .Bind(out childItemsCollection)
                .Sort(Comparer<SelectableItem<ContactDetails>>.Create((first, second) =>
                {
                    return String.Compare(first.Item.FullName, second.Item.FullName, StringComparison.Ordinal);
                }))
                .Subscribe();

            DisposableItems.Add(subscriptions);

            contactsGroup.GroupChilds = childItemsCollection;
            return contactsGroup;
        }
        
        private void ContactPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectableItem<ContactDetails>.IsSelected))
                return;

            var item = sender as SelectableItem<ContactDetails>;
            
            if (item.IsSelected && !ContactsToImport.Contains(item.Item)) 
                ContactsToImport.Add(item.Item);
            else if (!item.IsSelected && ContactsToImport.Contains(item.Item))
                ContactsToImport.Remove(item.Item);

            RaisePropertyChanged(() => HasAnyContactsToImport);
        }
  
        public ObservableCollection<ContactDetails> ContactsToImport { get; } = new ObservableCollection<ContactDetails>();

        protected override async Task<IEnumerable<SelectableItem<ContactDetails>>> LoadData(int offset)
        {
            var phoneContacts = await _phoneContactsService.GetPhoneContacts();
            return phoneContacts.Select(x => new SelectableItem<ContactDetails>(x) { IsSelectionEnabled = true });
        }

        public MvxCommand<SelectableItem<ContactDetails>> ChildItemTapped => new MvxExceptionGuardedCommand<SelectableItem<ContactDetails>>((item) =>
            {
                item.IsSelected = !item.IsSelected; 
            });
        
        public MvxCommand<ContactDetails> AvatarTapped => new MvxExceptionGuardedCommand<ContactDetails>(item =>
        {
            var selectedItem = Items.SelectMany(x => x.GroupChilds).Select(x => x as SelectableItem<ContactDetails>).FirstOrDefault(x => x.Item == item);
            if (selectedItem != null)
                selectedItem.IsSelected = !selectedItem.IsSelected;
        });
        
        public MvxCommand Close => new MvxExceptionGuardedCommand(() =>
            {
                ServicesLocation.NavigationService.Close(this);
            });

        public MvxCommand Import => new ImportContactsCommandBuilder(this, _phoneContactsService).BuildCommand();
        public MvxCommand SelectAll => new MvxExceptionGuardedCommand(() =>
        {
            if (Items == null)
                return;
            
            foreach (var item in Items.SelectMany(x => x.GroupChilds).Select(x => x as SelectableItem<ContactDetails>))
                item.IsSelected = true;
        });
        
        public MvxCommand ClearSelection => new MvxExceptionGuardedCommand(() =>
        {
            if (Items == null)
                return;
            
            foreach (var item in Items.SelectMany(x => x.GroupChilds).Select(x => x as SelectableItem<ContactDetails>))
                item.IsSelected = false;
        });

        public string RecentSearchWord { get; set; } = string.Empty;
        
        public MvxCommand<string> SearchCommand => new MvxExceptionGuardedCommand<string>((searchQuery) =>
        {
            RecentSearchWord = searchQuery ?? string.Empty;
            OnSearchQueryChanged();
            
            foreach (var item in Items.SelectMany(x => x.GroupChilds).Select(x => x as SelectableItem<ContactDetails>))
                item.Item.RaiseContactNamePropertyChanged();
        });

        public bool AreAdsAvailable => _premiumService.AreAdsAvailable;
         

        public void SetContactsToImport(IEnumerable<ContactDetails> contactDetailsList)
        {
            ContactsToImport.Clear();
            ContactsToImport.AddRange(contactDetailsList);
        }
    }
}