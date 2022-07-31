using System;
using System.Collections.Generic;
using System.Linq;
using Debts.Commands;
using Debts.Commands.Budget;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.Messenging;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Model.NavigationData;
using Debts.Resources;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Settings;
using Debts.ViewModel.Contacts;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Commands;

namespace Debts.ViewModel.Budget
{
    public class AddBudgetViewModel : BaseViewModel<AddBudgetItemNavigationData>
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly SettingsService _settingsService;
        private readonly AdvertisementService _advertisementService;

        public AddBudgetViewModel(QueryCommandExecutor queryCommandExecutor,
            SettingsService settingsService,
            AdvertisementService advertisementService)
        {
            _queryCommandExecutor = queryCommandExecutor;
            _settingsService = settingsService;
            _advertisementService = advertisementService;
            
            MessageObserversController.AddObservers(new InvokeActionMessageObserver<BudgetCategorySelectedMvxMessage>(msg =>
            {
                SelectedCategory = msg.BudgetCategory;
            }));
            
            Currency = _settingsService.GetDefaultCurrency();
            AvailableCurrencies = _settingsService.GetCurrencyList().Select(x => x.Currency).ToList();
        }

        public override void Prepare(AddBudgetItemNavigationData parameter)
        {
            base.Prepare(parameter);
            Type = parameter.Type;
            
            if (_advertisementService.AreAdsAvailable)
                _advertisementService.PreloadFullScreenAd();    
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            ServicesLocation.MessageQueue.ResendMessages<BudgetCategorySelectedMvxMessage>();
        }

        public BudgetCategory SelectedCategory { get; set; }
        
        public string Title { get; set; }
        
        public BudgetType Type { get; set; }

        public IEnumerable<BudgetType> AvailableTypes { get; } = new List<BudgetType>()
        {
            BudgetType.Expense,
            BudgetType.Income
        };
        
        public decimal? Amount { get; set; }

        public string Currency { get; set; }
        
        public IEnumerable<string> AvailableCurrencies { get; private set; }
        
        public MvxCommand Add => new MvxExceptionGuardedCommand(() =>
        {
            int targetPage = -1;
            string errorMsg = string.Empty;
            if (string.IsNullOrEmpty(Title))
            {
                targetPage = 0;
                errorMsg = TextResources.ViewModel_AddFinanceOperationViewModel_OperationTitleMissing;
            }
            else if (SelectedCategory == null)
            {
                targetPage = 1;
                errorMsg = TextResources.ViewModel_AddBudgetViewModel_CategoryMissing;
            } 
            else if (!Amount.HasValue || Amount <= 0)
            {
                targetPage = 2;
                errorMsg = TextResources.ViewModel_AddFinanceOperationViewModel_OperationAmountMissing;
            }
            
            
            if (!string.IsNullOrEmpty(errorMsg))
            {
                CurrentSubPage = targetPage;
                ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, errorMsg)
                    {Style = ToastMvxMessage.ToastStyle.Error});
                return;
            } 
            
            AddBudget.Execute();
        });

        public MvxCommand AddBudget => new AddNewBudgetCommandBuilder(this, _queryCommandExecutor).BuildCommand();

        public MvxCommand Close => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.NavigationService.Close(this);
            
            if (_advertisementService.AreAdsAvailable)
                _advertisementService.ShowFullScreenAd();
        });

        public int CurrentSubPage { get; set; } = 0;

        public MvxCommand Next => new MvxExceptionGuardedCommand(() =>
        {
            string errorMsg = string.Empty;
            if (CurrentSubPage == 0 && string.IsNullOrEmpty(Title))
                errorMsg = TextResources.ViewModel_AddFinanceOperationViewModel_EnterOperationTitle;
            else if (CurrentSubPage == 1 && SelectedCategory == null)
                errorMsg = TextResources.ViewModel_AddBudgetViewModel_CategoryMissing;
            else if (CurrentSubPage == 2 && (!Amount.HasValue || Amount <= 0))
                errorMsg = TextResources.ViewModel_AddFinanceOperationViewModel_EnterOperationAmount; 
            
            if (!string.IsNullOrEmpty(errorMsg))
                ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, errorMsg) { Style = ToastMvxMessage.ToastStyle.Error });
            else
                CurrentSubPage = Math.Min(CurrentSubPage+1, 3);
        });

        public MvxCommand PickCategory => new MvxExceptionGuardedCommand(() =>
            {
                ServicesLocation.NavigationService.Navigate<PickBudgetCategoryViewModel>();
            });
    }
}