using System;
using System.Collections.Generic;
using System.Linq;
using Debts.Commands;
using Debts.Commands.Finances;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.Messenging;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Model.NavigationData;
using Debts.Resources;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.LocationService;
using Debts.Services.Settings;
using Debts.ViewModel.Contacts;
using MvvmCross.Commands;

namespace Debts.ViewModel.Finances
{
    public class AddFinanceOperationViewModel : BaseViewModel<AddFinanceOperationNavigationData>
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly LocationService _locationService;
        private readonly PermissionService _permissionService;
        private readonly SettingsService _settingsService;
        private readonly AdvertisementService _advertisementService;

        public AddFinanceOperationViewModel(QueryCommandExecutor queryCommandExecutor, 
            LocationService locationService, 
            PermissionService permissionService, 
            SettingsService settingsService,
            AdvertisementService advertisementService)
        {
            _queryCommandExecutor = queryCommandExecutor;
            _locationService = locationService;
            _permissionService = permissionService;
            _settingsService = settingsService;
            _advertisementService = advertisementService;

            MessageObserversController.AddObservers(new InvokeActionMessageObserver<ContactSelectedMvxMessage>(msg =>
            {
                PickedContact = msg.SelectedContact;
            }));
            Currency = _settingsService.GetDefaultCurrency();
            AvailableCurrencies = _settingsService.GetCurrencyList().Select(x => x.Currency).ToList();
        }
        
        public override void Prepare(AddFinanceOperationNavigationData parameter)
        {
            base.Prepare(parameter);
            Type = parameter.Type;
            
            if (_advertisementService.AreAdsAvailable)
                _advertisementService.PreloadFullScreenAd();    
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            ServicesLocation.MessageQueue.ResendMessages<ContactSelectedMvxMessage>();
        }
        

        public string Title { get; set; }
        
        public ContactDetails PickedContact { get; set; }
        
        public FinanceOperationType Type { get; set; }

        public IEnumerable<FinanceOperationType> AvailableTypes { get; } = new List<FinanceOperationType>()
        {
            FinanceOperationType.Debt,
            FinanceOperationType.Loan
        };

        public DateTime? Deadline { get; set; }

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
                } else if (PickedContact == null)
                {
                    targetPage = 1;
                    errorMsg = TextResources.ViewModel_AddFinanceOperationViewModel_ContactMissing;
                } else if (!Amount.HasValue || Amount <= 0)
                {
                    targetPage = 2;
                    errorMsg = TextResources.ViewModel_AddFinanceOperationViewModel_OperationAmountMissing;
                } else if (!Deadline.HasValue)
                {
                    targetPage = 3;
                    errorMsg = TextResources.ViewModel_AddFinanceOperationViewModel_PaymentDateMissing;
                }
 
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    CurrentSubPage = targetPage;
                    ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, errorMsg) { Style = ToastMvxMessage.ToastStyle.Error });
                    return;
                }
                
                AddFinanceOperation.Execute();
            });

        private MvxCommand AddFinanceOperation => new AddNewFinanceOperationCommandBuilder(
                this,
                    _queryCommandExecutor, 
                    _locationService, 
                    _permissionService)
                .BuildCommand();
        
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
            else if (CurrentSubPage == 1 && PickedContact == null)
                errorMsg = TextResources.ViewModel_AddFinanceOperationViewModel_EnterContact;
            else if (CurrentSubPage == 2 && (!Amount.HasValue || Amount <= 0))
                errorMsg = TextResources.ViewModel_AddFinanceOperationViewModel_EnterOperationAmount;
            else if (CurrentSubPage == 3 && !Deadline.HasValue)
                errorMsg = TextResources.ViewModel_AddFinanceOperationViewModel_EnterPaymentDate;
            
            if (!string.IsNullOrEmpty(errorMsg))
                ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, errorMsg) { Style = ToastMvxMessage.ToastStyle.Error });
            else
                CurrentSubPage = Math.Min(CurrentSubPage+1, 3);
        });
        
        public MvxCommand PickDate => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.Messenger.Publish(new PickDateMvxMessage(this));
        });
        
        public MvxCommand PickContact => new MvxExceptionGuardedCommand(() =>
            {
                ServicesLocation.NavigationService.Navigate<PickContactViewModel>();
            });
    }
}