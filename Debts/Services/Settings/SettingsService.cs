using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Model;

namespace Debts.Services.Settings
{
    public class SettingsService
    {
        private readonly IStorageService _storageService;
        private readonly CurrencyProvider _currencyProvider;
        private readonly IDisplayNameProvider _displayNameProvider;
        private const string DisplayNameKey = "DisplayNameKey";
        private const string DefaultCurrencyKey = "DefaultCurrencyKey";

        private const string DefaultMinimalAmountOfTimeBeforeNotificationsKey =
            "DefaultMinimalAmountOfTimeBeforeNotificationsKey";
        private const string DefaultMinimalAmountOfTimePassedNotificationsKey =
            "DefaultMinimalAmountOfTimePassedNotificationsKey";

        private const string UpcomingDebtsNotificationsKey = "UpcomingDebtsNotificationsKey";
        private const string UpcomingLoansNotificationsKey = "UpcomingLoansNotificationsKey";
        private const string UnpaidDebtsNotificationsKey = "UnpaidDebtsNotificationsKey";
        private const string UnpaidLoansNotificationsKey = "UnpaidLoansNotificationsKey";

        
        public SettingsService(IStorageService storageService, CurrencyProvider currencyProvider, IDisplayNameProvider displayNameProvider)
        {
            _storageService = storageService;
            _currencyProvider = currencyProvider;
            _displayNameProvider = displayNameProvider;
        } 
        
        public async Task<string> GetMessageForReminder(FinanceOperation ofFinanceOperation)
        {
            return "Finance operation: " + ofFinanceOperation.FinancePrimaryId;
        }

        public string GetShareMessageTemplate(FinanceOperation operation)
        {
            return "Hey!" + Environment.NewLine +
                   $"I would like to remind you about finance operation - {operation.Title}, {operation.PaymentDetails.Amount} {operation.PaymentDetails.Currency}." + Environment.NewLine + Environment.NewLine;
        }

        public string GetDefaultCurrency()
        {
            return GetDefaultCurrencyModel().Currency;
        }

        public void SetDefaultCurrency(CurrencyModel currencyModel)
        {
            _storageService.Store(DefaultCurrencyKey, currencyModel);
        }

        public CurrencyModel GetDefaultCurrencyModel()
        {
            if (_storageService.Contains(DefaultCurrencyKey))
                return _storageService.Get<CurrencyModel>(DefaultCurrencyKey);

            return _currencyProvider
                .GetCurrencyList()
                .First(x => x.Currency == "USD");
        }

        public IEnumerable<CurrencyModel> GetCurrencyList() => _currencyProvider.GetCurrencyList();

        public TimeSpan MinimalAmountOfTimeBeforeUpcomingNotifications()
        {
            if (_storageService.Contains(DefaultMinimalAmountOfTimeBeforeNotificationsKey))
                return _storageService.Get<TimeSpan>(DefaultMinimalAmountOfTimeBeforeNotificationsKey);
            
            return TimeSpan.FromDays(3);
        }

        public void SetMinimalAmountOfTimeBeforeNotifications(TimeSpan timeSpan)
        {
            _storageService.Store(DefaultMinimalAmountOfTimeBeforeNotificationsKey, timeSpan);
        }
        
        public TimeSpan MinimalAmountOfTimeAfterDeadlineExceedNotifications()
        {
            if (_storageService.Contains(DefaultMinimalAmountOfTimePassedNotificationsKey))
                return _storageService.Get<TimeSpan>(DefaultMinimalAmountOfTimePassedNotificationsKey);
            
            return TimeSpan.FromDays(3);
        }

        public void SetMinimalAmountOfTimeAfterDeadlineExceedNotifications(TimeSpan timeSpan)
        {
            _storageService.Store(DefaultMinimalAmountOfTimePassedNotificationsKey, timeSpan);
        }

        public IEnumerable<TimeSpan> GetMinimalAmountOfTimeBeforeNotificationsList()
        {
            yield return TimeSpan.FromDays(1);
            yield return TimeSpan.FromDays(2);
            yield return TimeSpan.FromDays(3);
            yield return TimeSpan.FromDays(5);
            yield return TimeSpan.FromDays(7);
            yield return TimeSpan.FromDays(14);
            yield return TimeSpan.FromDays(31);
        }

        public bool UpcomingDebtsNotificationsEnabled
        {
            get => _storageService.Get(UpcomingDebtsNotificationsKey, true);
            set => _storageService.Store(UpcomingDebtsNotificationsKey, value);
        }
        
        public bool UpcomingLoansNotificationsEnabled
        {
            get => _storageService.Get(UpcomingLoansNotificationsKey, true);
            set => _storageService.Store(UpcomingLoansNotificationsKey, value);
        }
        
        public bool UnpaidDebtsNotificationsEnabled
        {
            get => _storageService.Get(UnpaidDebtsNotificationsKey, true);
            set => _storageService.Store(UnpaidDebtsNotificationsKey, value);
        }
        
        public bool UnpaidLoansNotificationsEnabled
        {
            get => _storageService.Get(UnpaidLoansNotificationsKey, true);
            set => _storageService.Store(UnpaidLoansNotificationsKey, value);
        }

        public string DisplayName
        {
            get => _storageService.Get(DisplayNameKey, _displayNameProvider.GetDefaultDisplayName());
            set => _storageService.Store(DisplayNameKey, value);
        }

        public NotificationSettings GetNotificationSettings()
        {
            return new NotificationSettings()
            {
                MinimalAmountOfTimeBeforeUpcomingNotifications = MinimalAmountOfTimeBeforeUpcomingNotifications(),
                MinimalAmountOfTimeAfterDeadlineExceedNotifications = MinimalAmountOfTimeAfterDeadlineExceedNotifications(),
                UpcomingDebtsNotificationsEnabled = UpcomingDebtsNotificationsEnabled,
                UpcomingLoansNotificationsEnabled = UpcomingLoansNotificationsEnabled,
                UnpaidDebtsNotificationsEnabled = UnpaidDebtsNotificationsEnabled,
                UnpaidLoansNotificationsEnabled = UnpaidLoansNotificationsEnabled
            };
        }
    }
}