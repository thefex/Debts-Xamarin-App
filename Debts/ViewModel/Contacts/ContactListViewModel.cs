using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Commands.ContactDetails;
using Debts.Commands.Contacts;
using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Contacts;
using Debts.Messenging;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Model;
using Debts.Resources;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Contacts;
using Debts.Services.Phone;
using Debts.Services.Settings;
using Debts.ViewModel.Finances;
using DynamicData;
using MvvmCross;
using MvvmCross.Commands;

namespace Debts.ViewModel.Contacts
{
    public class ContactListViewModel : ListViewModel<string, ContactDetails, ContactsGroup>
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly ImportContactsServices _importContactsServices;
        private readonly WalkthroughService _walkthroughService;
        private readonly PremiumService _premiumService;
        private readonly AdvertisementService _advertisementService;
        readonly Subject<string> _searchByWordSubject = new Subject<string>();

        public ContactListViewModel(QueryCommandExecutor queryCommandExecutor,
            ImportContactsServices importContactsServices,
            WalkthroughService walkthroughService,
            PremiumService premiumService,
            AdvertisementService advertisementService)
        {
            _queryCommandExecutor = queryCommandExecutor;
            _importContactsServices = importContactsServices;
            _walkthroughService = walkthroughService;
            _premiumService = premiumService;
            _advertisementService = advertisementService;

            MessageObserversController.AddObservers(
                new InvokeActionMessageObserver<ItemPublishedMessage<ContactDetails>>(
                    msg => ItemsSource.Add(msg.Item))
                )
                .AddObservers(
                    new InvokeActionMessageObserver<ItemPublishedMessage<IEnumerable<ContactDetails>>>(
                        msg =>
                        {
                            ItemsSource.Edit((x) =>
                            {
                                var itemsToAdd = msg.Item.Except(x);
                                ItemsSource.AddRange(itemsToAdd);
                            });
                        })
                )
                .AddObservers(new InvokeActionMessageObserver<ItemRemovedMessage<ContactDetails>>(msg =>
                {
                    ItemsSource.Remove(msg.Item);
                }));
            SubscribeToPremiumStateUpdatedIfPossible();
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            ServicesLocation.MessageQueue.ResendAndClearMessages<ItemPublishedMessage<IEnumerable<ContactDetails>>>();
            SubscribeToPremiumStateUpdatedIfPossible();
            if (_advertisementService.AreAdsAvailable)
                _advertisementService.PreloadFullScreenAd();
        } 
         
        private bool isSubscribedToPremiumStateUpdate = false;
        private void SubscribeToPremiumStateUpdatedIfPossible()
        {
            if (isSubscribedToPremiumStateUpdate)
                return;
            isSubscribedToPremiumStateUpdate = true;
            _premiumService.PremiumStateUpdated += PremiumServiceOnPremiumStateUpdated;
        }

        private void PremiumServiceOnPremiumStateUpdated()
        {
            RaisePropertyChanged(() => AreAdsAvailable);
            RaisePropertyChanged(() => Items);
        }

        public override void ViewDisappeared()
        {
            _premiumService.PremiumStateUpdated -= PremiumServiceOnPremiumStateUpdated;
            isSubscribedToPremiumStateUpdate = false;
        }

        public override async Task Load()
        {
            await base.Load();
            if (!await _importContactsServices.PhoneContactsService.CheckHasImportedContactsInPast() && _walkthroughService.IsContactListTutorialShown())
            {
                ServicesLocation.Messenger.Publish(
                    new QuestionMessageWithCheckBoxMvxMessage(
                        TextResources.ViewModel_ContactListViewModel_Dialog_ImportContacts_Title,
                        TextResources.ViewModel_ContactListViewModel_Dialog_ImportContacts_Content,
                        this)
                    {
                        OnYes = (neverAskAgain) =>
                        {
                            ImportContacts.Execute();
                        },
                        OnNo = (neverAskAgain) =>
                        {
                            if (neverAskAgain)
                                _importContactsServices.PhoneContactsService.SetContactsAsImported();
                        }
                    });
            }
        }

        protected override bool BuildAndSetObservable()
        {
            var result = base.BuildAndSetObservable();

            if (result)
            {
                var searchEventObservable = _searchByWordSubject
                    .DistinctUntilChanged()
                    .SelectMany(x => Observable.FromAsync(() => LoadData(0)))
                    .ObserveOn(SynchronizationContext.Current)
                    .Subscribe(items =>
                    {
                        ItemsSource.Edit(updateAction =>
                        {
                            updateAction.Clear();
                            updateAction.AddRange(items);
                        });
                    });
                DisposableItems.Add(searchEventObservable);
            }
            
            return result;
        }

        public string RecentSearchWord { get; set; }
 
        protected IListDataQuery<ContactDetails> BuildQuery(ContactListQueryParameter limitOffsetQueryParameter) => new GetAllContactsQuery(limitOffsetQueryParameter);

        protected override ISourceList<ContactDetails> ItemsSource { get; } = new SourceList<ContactDetails>();

        protected override IObservable<IChangeSet<ContactsGroup>> GetObservableChangeSet() =>
            ItemsSource.Connect()
                .Transform(contact => new SelectableItem<ContactDetails>(contact) {IsSelectionEnabled = false})
                .Sort(Comparer<SelectableItem<ContactDetails>>.Create( (x,y) => String.Compare(x.Item.FullName, y.Item.FullName, StringComparison.Ordinal)))
                .GroupOn(x =>
                {
                    var firstCharacter = (x.Item.FullName.Trim().FirstOrDefault());
                    if (firstCharacter == '\0')
                        firstCharacter = ' ';

                    return firstCharacter.ToString().ToUpper();
                })
                .Transform(CreateContactsGroup)
                .Sort(Comparer<ContactsGroup>.Create( (x,y) => String.Compare(x.GroupingLetter, y.GroupingLetter, StringComparison.Ordinal)))
        ;

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
        
        protected override Task<IEnumerable<ContactDetails>> LoadData(int offset)
          => _queryCommandExecutor.Execute(BuildQuery(
            new ContactListQueryParameter()
            {
                Limit = TakeItemsCount,
                Offset = offset,
                SearchQuery = RecentSearchWord
            }));

        public MvxCommand<SelectableItem<ContactDetails>> ChildItemTapped => new MvxExceptionGuardedCommand<SelectableItem<ContactDetails>>((item) =>
            {
                ServicesLocation.NavigationService.Navigate<ContactDetailsViewModel, ContactDetails>(item.Item);
            });

        public MvxCommand ImportContacts => new TransferToImportContactsCommandBuilder(
            this, _importContactsServices.PermissionService, _importContactsServices.PhoneContactsService
        ).BuildCommand();
        
            
        public MvxCommand<string> SearchCommand => new MvxExceptionGuardedCommand<string>((searchQuery) =>
        {
            RecentSearchWord = searchQuery ?? string.Empty;
            _searchByWordSubject.OnNext(RecentSearchWord);
            foreach (var item in Items.SelectMany(x => x.GroupChilds))
            {
                var contactDetail = item as SelectableItem<ContactDetails>;
                contactDetail.Item.RaiseContactNamePropertyChanged();
            }
        });
        
        public MvxCommand<SelectableItem<ContactDetails>> Call => new MvxExceptionGuardedCommand<SelectableItem<ContactDetails>>(
            x =>
            {
                new CallContactAsyncGuardedCommandBuilder(
                    Mvx.IoCProvider.Resolve<PhoneCallServices>(),
                    x.Item,
                    _premiumService).BuildCommand().Execute();
            });

        public MvxCommand<SelectableItem<ContactDetails>> SMS => new MvxExceptionGuardedCommand<SelectableItem<ContactDetails>>(
            x =>
            {
                new SendMessageToContactAsyncGuardedCommandBuilder(x.Item).BuildCommand().Execute();
            });
        
        public bool AreAdsAvailable => _premiumService.AreAdsAvailable;
    }
}