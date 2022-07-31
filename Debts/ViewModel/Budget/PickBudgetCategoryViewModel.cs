using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Budget;
using Debts.Messenging.Messages.App;
using Debts.Services;
using Debts.Services.AppGrowth;
using DynamicData;
using MvvmCross.Commands;

namespace Debts.ViewModel.Budget
{
    public class PickBudgetCategoryViewModel : ListViewModel<string, BudgetCategory>
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly PremiumService _premiumService;
        readonly Subject<string> _searchByWordSubject = new Subject<string>();

        public PickBudgetCategoryViewModel(QueryCommandExecutor queryCommandExecutor, PremiumService premiumService)
        {
            _queryCommandExecutor = queryCommandExecutor;
            _premiumService = premiumService;
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

        
        protected override ISourceList<BudgetCategory> ItemsSource { get; } = new SourceList<BudgetCategory>();
        protected override IObservable<IChangeSet<BudgetCategory>> GetObservableChangeSet() => ItemsSource.Connect();

        protected override Task<IEnumerable<BudgetCategory>> LoadData(int startIndex) => _queryCommandExecutor.Execute(new BudgetCategoriesQuery()
        {
            SearchQuery = RecentSearchWord
        });

        public override MvxCommand<BudgetCategory> ItemTapped => new MvxExceptionGuardedCommand<BudgetCategory>((item) =>
        {
            ServicesLocation.NavigationService.Close(this);
            ServicesLocation.Messenger.Publish(new BudgetCategorySelectedMvxMessage(this, item)); 
        }); 
        
        public MvxCommand Close => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.NavigationService.Close(this);
        });

        public bool AreAdsAvailable => _premiumService.AreAdsAvailable;
          
        public string RecentSearchWord { get; set; }

        public MvxCommand<string> SearchCommand => new MvxExceptionGuardedCommand<string>((searchQuery) =>
        {
            RecentSearchWord = searchQuery ?? string.Empty;
            _searchByWordSubject.OnNext(RecentSearchWord);
                
            foreach(var item in Items)
                item?.RaiseNameChanged();
        });
    }
}