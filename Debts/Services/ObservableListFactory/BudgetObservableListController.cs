using System;
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
    public class BudgetObservableListController
    {
        private readonly IList<IDisposable> _disposableSubscriptions;
        private readonly ISourceList<BudgetItem> _itemsSource;
        readonly Subject<string> _searchByWordSubject = new Subject<string>();
        readonly Subject<Unit> _filterSubject = new Subject<Unit>();
 
        public BudgetObservableListController(IList<IDisposable> disposableSubscriptions, ISourceList<BudgetItem> itemsSource)
        {
            _disposableSubscriptions = disposableSubscriptions;
            _itemsSource = itemsSource;
        }
        
        public SynchronizationContext SynchronizationContext { get; set; }

        public IObservable<IChangeSet<BudgetItemGroup>> GetObservableChangeSet()
        {
            return _itemsSource
                .Connect()
                .Sort(Comparer<BudgetItem>.Create((first, second) =>
                {
                    return first.CreatedAt.CompareTo(second.CreatedAt);
                }))
                .GroupOn(GetGroupType)
                .Transform(CreateBudgetItemGroup);
        } 

        private DateTime GetGroupType(BudgetItem forBudget) => forBudget.CreatedAt.Date;

        private BudgetItemGroup CreateBudgetItemGroup(IGroup<BudgetItem, DateTime> budgetGroup)
        {
            var group = new BudgetItemGroup()
            {
                ForDate = budgetGroup.GroupKey
            };
            
            ReadOnlyObservableCollection<BudgetItem> childItemsCollection;

            var groupChangeSet = budgetGroup.List.Connect();
  
            var subscriptions = groupChangeSet
                .ObserveOn(SynchronizationContext)
                .Bind(out childItemsCollection)
                .Sort(Comparer<BudgetItem>.Create((first, second) => (first.CreatedAt.CompareTo(second.CreatedAt))))
                .Subscribe();

            _disposableSubscriptions.Add(subscriptions);

            group.GroupChilds = childItemsCollection;
            return group;
        }
        
        public void BuildObservableSet(Func<int, Task<IEnumerable<BudgetItem>>> loadData)
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