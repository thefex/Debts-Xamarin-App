using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Commands.Budget;
using Debts.Commands.FinanceDetails;
using Debts.Commands.Finances;
using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Budget;
using Debts.Messenging;
using Debts.Messenging.Messages.App;
using Debts.Model;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.LocationService;
using Debts.Services.ObservableListFactory;
using Debts.ViewModel.AppGrowth;
using Debts.ViewModel.Finances;
using DynamicData;
using MvvmCross.Commands;

namespace Debts.ViewModel.Budget
{
    public class BudgetListViewModel : ListViewModel<string, BudgetItem, BudgetItemGroup>
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly PremiumService _premiumService;
        readonly BudgetObservableListController _budgetObservableListController;
 
        public BudgetListViewModel(QueryCommandExecutor queryCommandExecutor, PremiumService premiumService)
        {
            _queryCommandExecutor = queryCommandExecutor;
            _premiumService = premiumService;
            _budgetObservableListController = new BudgetObservableListController(DisposableItems, ItemsSource);

            MessageObserversController.AddObservers(
                    new InvokeActionMessageObserver<ItemPublishedMessage<BudgetItem>>(msg =>
                    {
                        if (ShouldAddNewBudgetItem(msg.Item))
                            ItemsSource.Add(msg.Item);
                    }))
                .AddObservers(new InvokeActionMessageObserver<ItemRemovedMessage<BudgetItem>>(msg =>
                {
                    ItemsSource.Remove(msg.Item);
                }));
            
            SubscribeToPremiumStateUpdatedIfPossible();
        }

        public bool AreAdsAvailable => _premiumService.AreAdsAvailable;
        
        public override void ViewAppeared()
        {
            base.ViewAppeared();
            SubscribeToPremiumStateUpdatedIfPossible();
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

        protected override bool BuildAndSetObservable()
        {
            var isObservableBuilt = base.BuildAndSetObservable();
            if (isObservableBuilt) 
                _budgetObservableListController.BuildObservableSet(LoadData);

            return isObservableBuilt;
        }
          
        protected virtual bool ShouldAddNewBudgetItem(BudgetItem budgetItem) => true;

        protected IListDataQuery<BudgetItem> BuildQuery(BudgetItemsQueryParameter limitOffsetQueryParameter) => new BudgetGetAllQuery(limitOffsetQueryParameter);
        
        protected sealed override ISourceList<BudgetItem> ItemsSource { get; } = new SourceList<BudgetItem>();
        protected override IObservable<IChangeSet<BudgetItemGroup>> GetObservableChangeSet() 
            => _budgetObservableListController.GetObservableChangeSet();

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            _budgetObservableListController.SynchronizationContext = SynchronizationContext.Current;
        }
 
        protected override Task<IEnumerable<BudgetItem>> LoadData(int startIndex) => _queryCommandExecutor.Execute(BuildQuery(
            new BudgetItemsQueryParameter()
            {
                Limit = TakeItemsCount,
                Offset = startIndex,
                SearchQuery = RecentSearchWord,
                StartDate = FilterStartDate,
                EndDate = FilterEndDate,
                CategoryId = SelectedFilterCategory?.MainCategoryId.ToString() ?? string.Empty
            }));
        
        public DateTime? FilterStartDate { get; set; } = new DateTime(DateTime.UtcNow.Date.Year, DateTime.UtcNow.Month, 1);
        
        public DateTime? FilterEndDate { get; set; } = new DateTime(DateTime.UtcNow.Date.Year, DateTime.UtcNow.Month, 1).AddMonths(1).Subtract(TimeSpan.FromMinutes(1));

        public DateTime?[] FilterDates => new[] {FilterStartDate, FilterEndDate};

        public bool HasDateFilter => FilterStartDate.HasValue && FilterEndDate.HasValue;

        public bool HasCategoryFilter => SelectedFilterCategory != null;
        public bool HasAnyFilter => HasDateFilter || HasCategoryFilter;
         
        public string RecentSearchWord { get; set; }
 
        public MvxCommand<BudgetItem> Delete => new ListDeleteBudgetItemOperationCommand(ItemsSource, _queryCommandExecutor).BuildCommand();
        
        public MvxCommand FilterByDate => new MvxExceptionGuardedCommand(() =>
            {
                if (!_premiumService.HasPremium)
                {
                    ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
                    return;
                }
                
                ServicesLocation.NavigationService.Navigate<FilterBudgetByDateViewModel>();
            });
         
        public MvxCommand ResetFilter => new MvxExceptionGuardedCommand(() =>
        {
            FilterStartDate = null;
            FilterEndDate = null;
            SelectedFilterCategory = null;
            RaisePropertyChanged(() => FilterDates);
            RaisePropertyChanged(() => HasDateFilter);
            RaisePropertyChanged(() => HasCategoryFilter);
            RaisePropertyChanged(() => HasAnyFilter);
            Filter.Execute();
        });
        
        public MvxCommand Filter => new MvxExceptionGuardedCommand(() =>
            {
                _budgetObservableListController.OnFilter();
                RaisePropertyChanged(() => HasDateFilter);
                RaisePropertyChanged(() => SelectedFilterCategory);
                RaisePropertyChanged(() => HasCategoryFilter);
                RaisePropertyChanged(() => HasAnyFilter);
                RaisePropertyChanged(() => FilterDates);
            });
        
        public MvxCommand<string> SearchCommand => new MvxExceptionGuardedCommand<string>((searchQuery) =>
            {
                RecentSearchWord = searchQuery ?? string.Empty;
                _budgetObservableListController.OnSearch(RecentSearchWord);
                
                foreach(var item in Items.SelectMany(x => x.GroupChilds))
                    (item as BudgetItem)?.RaiseTitleAmountChanged();
            });

        public MvxCommand FilterByStatus => new MvxExceptionGuardedCommand(() =>
        {
            if (!_premiumService.HasPremium)
            {
                ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
                return;
            }
                
            ServicesLocation.NavigationService.Navigate<FilterBudgetByCategoryViewModel>();
        });

        public MvxCommand<BudgetItem> Details => new MvxExceptionGuardedCommand<BudgetItem>((x) =>
            {
                ServicesLocation.NavigationService.Navigate<BudgetDetailsViewModel, BudgetItem>(x);
            });

        public BudgetCategory SelectedFilterCategory { get; set; }
    }
}