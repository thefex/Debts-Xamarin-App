using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Services;
using DynamicData;
using MvvmCross.Binding.Extensions;
using MvvmCross.Commands;

namespace Debts.ViewModel
{
    public abstract class ListViewModel<TInitParams, TApiDataModel, TResultModel> : BaseViewModel<TInitParams> where TInitParams : class
    { 
        private bool isListLoading;
        private bool isLoadMoreDataEnabled = true;

        private ReadOnlyObservableCollection<TResultModel> _items;
        protected readonly IList<IDisposable> DisposableItems = new List<IDisposable>();
		
        public int ItemsCount => ItemsSource.Count;
        protected abstract ISourceList<TApiDataModel> ItemsSource { get; }
	  
        public ReadOnlyObservableCollection<TResultModel> Items
        {
            get => _items;
            private set => SetProperty(ref _items, value);
        }
	      
        public bool HasAnyItems => ItemsSource.Count > 0;

        public virtual MvxCommand<TResultModel> ItemTapped { get; } = new MvxExceptionGuardedCommand<TResultModel>(itemModel => { });

        protected virtual bool BuildAndSetObservable()
        {
            if (DisposableItems.Count > 0)
                return false;
            ReadOnlyObservableCollection<TResultModel> observableCollection = null;

            DisposableItems.Add(
                GetObservableChangeSet()
                    .ObserveOn(SynchronizationContext.Current)
                    .Bind(out observableCollection)
                    .Subscribe()
            );

            DisposableItems.Add(
                ItemsSource
                    .CountChanged
                    .ObserveOn(SynchronizationContext.Current)
                    .Subscribe(count => OnItemsCountChanged()));
			
            Items = observableCollection;
            return true;
        }

        protected virtual void OnItemsCountChanged()
        {
            RaisePropertyChanged(() => IsLoadingMoreDataEnabled);
            RaisePropertyChanged(() => ItemsCount);
            RaisePropertyChanged(() => HasAnyItems);
        }
	   

        protected abstract IObservable<IChangeSet<TResultModel>> GetObservableChangeSet();

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            BuildAndSetObservable();
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);

            foreach (var item in DisposableItems)
                item.Dispose();
            DisposableItems.Clear();
        }
        
        public override async System.Threading.Tasks.Task Load()
        { 
            await LoadLists();
            RaisePropertyChanged(() => IsLoadingMoreDataEnabled);
        }
        
        public bool IsLoadingMoreData { get; set; }

        public int TakeItemsCount => 25;

        public bool IsLoadingMoreDataEnabled => ItemsSource.Count >= TakeItemsCount && isLoadMoreDataEnabled; 
          
        protected abstract Task<IEnumerable<TApiDataModel>> LoadData(int startIndex);

        protected virtual async Task LoadLists()
        {
            try
            {
                var items = await LoadData(ItemsCount);
                EditListDiff(items);
                OnItemsCountChanged();
            } 
            catch (Exception e)
            {
                ServicesLocation.ExceptionGuard.OnException(e);
            }
            finally
            {
                IsListLoaded = true;
            }
        }
        public bool IsListLoaded
        {
            get => isListLoading;
            set => SetProperty(ref isListLoading, value);
        }
        
        protected virtual void EditListDiff(IEnumerable<TApiDataModel> items){
            ItemsSource.Edit(action =>
            {
                action.Clear();
                action.AddRange(items);
            });
        }
        
        public MvxCommand LoadMore => new MvxExceptionGuardedCommand(async () =>
        {
            try
            {
                IsLoadingMoreData = true;
                var loadedData = await LoadData(ItemsSource.Count);
                ItemsSource.AddRange(loadedData);

                if (loadedData.Count() < TakeItemsCount)
                {
                    isLoadMoreDataEnabled = false;
                    RaisePropertyChanged(() => IsLoadingMoreDataEnabled);
                }
            }
            finally
            {
                IsLoadingMoreData = false;
            }
        });
    }

    public abstract class ListViewModel<TInitParams, TDataModel> : ListViewModel<TInitParams, TDataModel, TDataModel> where TInitParams : class
    {
        
    }
}