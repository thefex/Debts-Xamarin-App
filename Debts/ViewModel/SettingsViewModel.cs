using System;
using System.Collections;
using System.Collections.Generic;
using Debts.Commands;
using Debts.Data;
using Debts.Messenging.Messages;
using Debts.Model;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Settings;
using Debts.ViewModel.AppGrowth;
using MvvmCross.Commands;
using Xamarin.Essentials;

namespace Debts.ViewModel
{
    public class SettingsViewModel : BaseViewModel<string>
    {
        private readonly SettingsService _settingsService;
        private readonly RateAppService _rateAppService;
        private readonly PremiumService _premiumService;

        public SettingsViewModel(SettingsService settingsService, RateAppService rateAppService,  PremiumService premiumService)
        {
            _settingsService = settingsService;
            _rateAppService = rateAppService;
            _premiumService = premiumService;
        }
        
        public override void Prepare()
        {
            base.Prepare();

            Currencies = _settingsService.GetCurrencyList();
            SelectedCurrency = _settingsService.GetDefaultCurrencyModel();
            MinimalAmountOfTimeBeforeNotifications = _settingsService.GetMinimalAmountOfTimeBeforeNotificationsList();
            SelectedMinimalAmountOfTimeBeforeNotifications = _settingsService.MinimalAmountOfTimeBeforeUpcomingNotifications();
            SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications = _settingsService.MinimalAmountOfTimeAfterDeadlineExceedNotifications();
            UpcomingDebtsNotificationsEnabled = _settingsService.UpcomingDebtsNotificationsEnabled;
            UpcomingLoansNotificationsEnabled = _settingsService.UpcomingLoansNotificationsEnabled;
            UnpaidDebtsNotificationsEnabled = _settingsService.UnpaidDebtsNotificationsEnabled;
            UnpaidLoansNotificationsEnabled = _settingsService.UnpaidLoansNotificationsEnabled;
        }

        public bool IsGoPremiumEnabled => _premiumService.PremiumState != PremiumState.PremiumOwnership;
        
        private CurrencyModel _selectedCurrency;
        public CurrencyModel SelectedCurrency
        {
            get => _selectedCurrency;
            set
            {
                if (_selectedCurrency != value)
                {
                    _selectedCurrency = value;
                    _settingsService.SetDefaultCurrency(_selectedCurrency);
                }
            }
        }

        public IEnumerable<CurrencyModel> Currencies { get; private set; }

        private TimeSpan _selectedMinimalAmountOfTimeBeforeNotifications;
        public TimeSpan SelectedMinimalAmountOfTimeBeforeNotifications
        {
            get => _selectedMinimalAmountOfTimeBeforeNotifications;
            set
            {
                if (_selectedMinimalAmountOfTimeBeforeNotifications != value)
                {
                    _selectedMinimalAmountOfTimeBeforeNotifications = value;
                    _settingsService.SetMinimalAmountOfTimeBeforeNotifications(value);
                }
            }
        }

        private TimeSpan _selectedMinimalAmountOfTimeAfterDeadlineExceedNotifications;

        public TimeSpan SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications
        {
            get => _selectedMinimalAmountOfTimeAfterDeadlineExceedNotifications;
            set
            {
                if (_selectedMinimalAmountOfTimeAfterDeadlineExceedNotifications != value)
                {
                    _selectedMinimalAmountOfTimeAfterDeadlineExceedNotifications = value;
                    _settingsService.SetMinimalAmountOfTimeAfterDeadlineExceedNotifications(value);
                }
            }
        }

        public IEnumerable<TimeSpan> MinimalAmountOfTimeBeforeNotifications { get; private set; }

        private bool upcomingDebtsNotificationsEnabled;
        public bool UpcomingDebtsNotificationsEnabled
        {
            get => upcomingDebtsNotificationsEnabled;
            set
            {
                if (upcomingDebtsNotificationsEnabled != value)
                {
                    upcomingDebtsNotificationsEnabled = value;
                    _settingsService.UpcomingDebtsNotificationsEnabled = value;
                }
            }
        }

        private bool upcomingLoansNotificationsEnabled;
        public bool UpcomingLoansNotificationsEnabled
        {
            get => upcomingLoansNotificationsEnabled;
            set
            {
                if (upcomingLoansNotificationsEnabled != value)
                {
                    upcomingLoansNotificationsEnabled = value;
                    _settingsService.UpcomingLoansNotificationsEnabled = value;
                }
            }
        }

        private bool unpaidDebtsNotificationsEnabled;
        public bool UnpaidDebtsNotificationsEnabled
        {
            get => unpaidDebtsNotificationsEnabled;
            set
            {
                if (unpaidDebtsNotificationsEnabled != value)
                {
                    unpaidDebtsNotificationsEnabled = value;
                    _settingsService.UnpaidDebtsNotificationsEnabled = value;
                }
            }
        }

        private bool unpaidLoansNotificationsEnabled;
        public bool UnpaidLoansNotificationsEnabled
        {
            get => unpaidLoansNotificationsEnabled;
            set
            {
                unpaidLoansNotificationsEnabled = value;
                _settingsService.UnpaidLoansNotificationsEnabled = value;
            }
        }
        public string DisplayName
        {
            get => _settingsService.DisplayName;
            set => _settingsService.DisplayName = value;
        }
        
        public MvxCommand RateApp => new MvxExceptionGuardedCommand(() =>
        {
            _rateAppService.LaunchAppRateProcess();
        });
        
        public MvxCommand GoPremium => new MvxExceptionGuardedCommand(() =>
            {
                ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
            });

        public MvxCommand RestorePurchase => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, "Restoring your purchases..."));
            _premiumService.RestorePurchase();
        });
        public MvxCommand PrivacyPolicy => new MvxExceptionGuardedCommand(() => { Browser.OpenAsync(
                new Uri("https://debts-finance-manage.flycricket.io/privacy.html")
            ); });
        
        public MvxCommand TermsOfUsage => new MvxExceptionGuardedCommand(() =>
            {
                Browser.OpenAsync(new Uri("http://debts-app.com/terms_and_conditions.html"));
            });
    }
}