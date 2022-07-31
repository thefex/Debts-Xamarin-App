using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Commands.FinanceDetails;
using Debts.Commands.Finances;
using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer;
using Debts.Messenging;
using Debts.Messenging.Messages.App;
using Debts.Model;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.ObservableListFactory;
using Debts.ViewModel.AppGrowth;
using Debts.ViewModel.Finances;
using DynamicData;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Commands;

namespace Debts.ViewModel.FinancesViewModel
{
    public abstract class BaseFinancesViewModel : ListViewModel<string, FinanceOperation, FinancesOperationGroup>
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly PremiumService _premiumService;
        private readonly AdvertisementService _advertisementService;
        readonly FinancesObservableListController _financesObservableListController;
 
        public BaseFinancesViewModel(QueryCommandExecutor queryCommandExecutor, PremiumService premiumService, AdvertisementService advertisementService)
        {
            _queryCommandExecutor = queryCommandExecutor;
            _premiumService = premiumService;
            _advertisementService = advertisementService;
            _financesObservableListController = new FinancesObservableListController(DisposableItems, ItemsSource);

            MessageObserversController.AddObservers(
                new InvokeActionMessageObserver<ItemPublishedMessage<FinanceOperation>>(msg =>
                {
                    if (ShouldAddNewFinanceOperation(msg.Item))
                        ItemsSource.Add(msg.Item);
                }))
                .AddObservers(new InvokeActionMessageObserver<ItemRemovedMessage<FinanceOperation>>(msg =>
                    {
                        ItemsSource.Remove(msg.Item);
                    }))
                .AddObservers(new InvokeActionMessageObserver<ItemUpdatedMessage<FinanceOperation>>(msg =>
                    {
                        var itemToUpdate = ItemsSource?.Items?.FirstOrDefault(x => x.FinancePrimaryId == msg.UpdatedItem.FinancePrimaryId);
                        if (itemToUpdate != null)
                        {
                            ItemsSource.Remove(itemToUpdate);
                            ItemsSource.Add(msg.UpdatedItem);
                        }
                    }));
            SubscribeToPremiumStateUpdatedIfPossible();
        }

        public bool AreAdsAvailable => _premiumService.AreAdsAvailable;

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            SubscribeToPremiumStateUpdatedIfPossible();
            
            if (_advertisementService.AreAdsAvailable)
                _advertisementService.PreloadFullScreenAd();
            if (_advertisementService.IsRewardAdAvailable)
                _advertisementService.PreloadRewardedAd();
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
                _financesObservableListController.BuildObservableSet(LoadData);

            return isObservableBuilt;
        }
          
        protected virtual bool ShouldAddNewFinanceOperation(FinanceOperation operation) => true;

        protected abstract IListDataQuery<FinanceOperation> BuildQuery(FinanceOperationsQueryParameter limitOffsetQueryParameter);
        
        protected override ISourceList<FinanceOperation> ItemsSource { get; } = new SourceList<FinanceOperation>();
        protected override IObservable<IChangeSet<FinancesOperationGroup>> GetObservableChangeSet() 
            => _financesObservableListController.GetObservableChangeSet();

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            _financesObservableListController.SynchronizationContext = SynchronizationContext.Current;
        }
 
        protected override async Task<IEnumerable<FinanceOperation>> LoadData(int startIndex)
        {
            return await _queryCommandExecutor.Execute(BuildQuery(
                new FinanceOperationsQueryParameter()
                {
                    Limit = TakeItemsCount,
                    Offset = startIndex,
                    IsActivePaymentEnabled = IsActivePaymentEnabled,
                    IsPaidOffPaymentEnabled = IsPaidOffPaymentEnabled,
                    IsPaymentDeadlineExceedEnabled = IsPaymentDeadlineExceedEnabled,
                    SearchQuery = RecentSearchWord,
                    StartDate = FilterStartDate,
                    EndDate = FilterEndDate
                }));
        }

        public DateTime? FilterStartDate { get; set; }
        
        public DateTime? FilterEndDate { get; set; }

        public DateTime?[] FilterDates => new[] {FilterStartDate, FilterEndDate};

        public bool HasDateFilter => FilterStartDate.HasValue && FilterEndDate.HasValue;
         
        public string RecentSearchWord { get; set; }

        public bool IsPaymentDeadlineExceedEnabled { get; set; } = true;

        public bool IsActivePaymentEnabled { get; set; } = true;

        public bool IsPaidOffPaymentEnabled { get; set; } = true;

        public MvxCommand<FinanceOperation> Delete => new ListDeleteFinanceOperationCommand(ItemsSource, _queryCommandExecutor).BuildCommand();
        public MvxCommand<FinanceOperation> Finalize => new ListFinalizeFinanceOperationCommand(ItemsSource, _queryCommandExecutor).BuildCommand();

        public MvxCommand<FinanceOperation> Details => new TransferToFinanceDetailsCommandBuilder(_queryCommandExecutor).BuildCommand();
        
        public MvxCommand FilterByDate => new MvxExceptionGuardedCommand(() =>
            {
                if (!_premiumService.HasPremium)
                {
                    ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
                    return;
                }
                
                ServicesLocation.NavigationService.Navigate<FilterFinancesByDateViewModel>();
            });
        
        public MvxCommand FilterByState => new MvxExceptionGuardedCommand(() =>
            {
                if (!_premiumService.HasPremium)
                {
                    ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
                    return;
                }
                
                ServicesLocation.NavigationService.Navigate<FilterFinancesByStateViewModel>();
            });
        
        public MvxCommand ResetDateFilter => new MvxExceptionGuardedCommand(() =>
        {
            FilterStartDate = null;
            FilterEndDate = null;
            Filter.Execute();
        });
        
        public MvxCommand Filter => new MvxExceptionGuardedCommand(() =>
            {
                _financesObservableListController.OnFilter();
                RaisePropertyChanged(() => HasDateFilter);
                RaisePropertyChanged(() => FilterDates);
            });
        
        public MvxCommand<string> SearchCommand => new MvxExceptionGuardedCommand<string>((searchQuery) =>
            {
                RecentSearchWord = searchQuery ?? string.Empty;
                _financesObservableListController.OnSearch(RecentSearchWord);
                
                foreach(var item in Items.SelectMany(x => x.GroupChilds))
                    (item as FinanceOperation)?.RaiseTitleAmountChanged();
            });
    }
}