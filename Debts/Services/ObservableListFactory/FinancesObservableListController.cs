using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Model;
using DynamicData;

namespace Debts.Services.ObservableListFactory
{
    public class FinancesObservableListController
    {
        private readonly IList<IDisposable> _disposableSubscriptions;
        private readonly ISourceList<FinanceOperation> _itemsSource;
        readonly Subject<string> _searchByWordSubject = new Subject<string>();
        readonly Subject<Unit> _filterSubject = new Subject<Unit>();
 
        public FinancesObservableListController(IList<IDisposable> disposableSubscriptions, ISourceList<FinanceOperation> itemsSource)
        {
            _disposableSubscriptions = disposableSubscriptions;
            _itemsSource = itemsSource;
        }
        
        public SynchronizationContext SynchronizationContext { get; set; }

        public IObservable<IChangeSet<FinancesOperationGroup>> GetObservableChangeSet()
        {
            return _itemsSource
                .Connect()
                .Sort(Comparer<FinanceOperation>.Create((first, second) =>
                    {
                        return (first.PaymentDetails.PaymentDate ?? first.PaymentDetails.DeadlineDate).CompareTo(second.PaymentDetails.PaymentDate ?? second.PaymentDetails.DeadlineDate);
                    }))
                .GroupOn(GetGroupType)
                .Transform(CreateFinanceOperationGroup)
                .Sort(Comparer<FinancesOperationGroup>.Create((x, y) => x.GroupType.CompareTo(y.GroupType)));
        } 

        private object GetGroupType(FinanceOperation forOperation)
        {
            if (forOperation.PaymentDetails.PaymentDate.HasValue)
                return FinanceGroupType.Paid;

            if (forOperation.PaymentDetails.DeadlineDate <= DateTime.UtcNow)
                return FinanceGroupType.DeadlinePassed;

            return forOperation.PaymentDetails.DeadlineDate.Date;
        }

        private FinancesOperationGroup CreateFinanceOperationGroup(IGroup<FinanceOperation, object> group)
        {
            bool isDateBasedGroup = (group.GroupKey is DateTime);
            
            var appointmentGroup = new FinancesOperationGroup()
            {
                GroupType = isDateBasedGroup ? FinanceGroupType.Active : (FinanceGroupType)group.GroupKey
            };

            if (group.GroupKey is DateTime dateTime)
                appointmentGroup.ForDate = dateTime;

            ReadOnlyObservableCollection<FinanceOperation> childItemsCollection;

            var groupChangeSet = group.List.Connect();
  
            var subscriptions = groupChangeSet
                .ObserveOn(SynchronizationContext)
                .Bind(out childItemsCollection)
                .Sort(Comparer<FinanceOperation>.Create((first, second) =>
                {
                    return (first.PaymentDetails.PaymentDate ?? first.PaymentDetails.DeadlineDate).CompareTo(second.PaymentDetails.PaymentDate ?? second.PaymentDetails.DeadlineDate);
                }))
                .Subscribe();

            _disposableSubscriptions.Add(subscriptions);

            appointmentGroup.GroupChilds = childItemsCollection;
            return appointmentGroup;
        }
        
        public void BuildObservableSet(Func<int, Task<IEnumerable<FinanceOperation>>> loadData)
        {
            var searchEventObservable = _searchByWordSubject
                .DistinctUntilChanged()
                .SelectMany(x => Observable.FromAsync(() => loadData(0)))
                .ObserveOn(SynchronizationContext)
                .Subscribe(items =>
                {
                    _itemsSource.Edit(updateAction =>
                    {
                        updateAction.Clear();
                        updateAction.AddRange(items);
                    });
                });

            var filterObservable = _filterSubject
                .SelectMany(x => Observable.FromAsync(() => loadData(0)))
                .ObserveOn(SynchronizationContext)
                .Subscribe(items =>
                {
                    _itemsSource.Edit(updateAction =>
                    {
                        updateAction.Clear();
                        updateAction.AddRange(items);
                    });
                });
    
            _disposableSubscriptions.Add(new CompositeDisposable(searchEventObservable, filterObservable));
        }

        public void OnSearch(string searchQuery) => 
            _searchByWordSubject.OnNext(searchQuery);

        public void OnFilter()
        {
            _filterSubject.OnNext(Unit.Default);
        }
    }
}