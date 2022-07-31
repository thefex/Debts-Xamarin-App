using System;
using System.Runtime.CompilerServices;
using Debts.ViewModel.AppGrowth;
using MvvmCross.Navigation;

namespace Debts.Services.AppGrowth
{
    public abstract class RateAppService
    {
        private const string MinimalDateToShowRateAppPromptKey = "MinimalDateToShowRateAppPromptKey";
        private const string IsAppRatedPromptKey = "IsAppRatedKey";
        private const string NumberOfTimesRateAppPromptShownKey = "NumberOfTimesRateAppPromptShownKey";
        private const string IsRateAppPromptEnabledKey = "IsRateAppPromptEnabledKey"; 
        
        private readonly IMvxNavigationService _navigationService;
        private readonly IStorageService _storageService;
        private readonly AppRelatedStatisticsService _appRelatedStatisticsService;

        public RateAppService(AppRelatedStatisticsService appRelatedStatisticsService,
                             IMvxNavigationService navigationService,
                             IStorageService storageService)
        {
            _navigationService = navigationService;
            _storageService = storageService;
            _appRelatedStatisticsService = appRelatedStatisticsService;
        }
        
        private bool ShouldPromptForAppRate()
        { 
            if (IsAppRated)
                return false;

            if (!IsAppRatePromptEnabled)
                return false;
            
            if (_appRelatedStatisticsService.TimePassedSinceInstallation < TimeSpan.FromDays(7))
                return false;

            if (_appRelatedStatisticsService.GetAppRunCounter() < 7)
                return false;

            var minimalDateToShowRateAppPrompt = _storageService.Get(MinimalDateToShowRateAppPromptKey, DateTime.UtcNow);
            return DateTime.UtcNow >= minimalDateToShowRateAppPrompt;
        }

        public bool IsAppRated => _storageService.Get(IsAppRatedPromptKey, false);

        public bool IsNeverShowAgainOptionAvailable => _storageService.Get(NumberOfTimesRateAppPromptShownKey, 0) > 1;

        public bool TryShowAppRatePrompt()
        {
            if (!ShouldPromptForAppRate())
                return false;
            
            IncreaseMinimalDateToShowAppRatePrompt();
            _navigationService.Navigate<RateAppViewModel>();

            void IncreaseMinimalDateToShowAppRatePrompt()
            {
                int numberOfTimesRateAppShown = _storageService.Get(NumberOfTimesRateAppPromptShownKey, 0);
                _storageService.Store(NumberOfTimesRateAppPromptShownKey, numberOfTimesRateAppShown+1);

                var minimalDateToShowRateAppPrompt = DateTime.UtcNow;

                if (numberOfTimesRateAppShown == 0)
                    minimalDateToShowRateAppPrompt = minimalDateToShowRateAppPrompt.AddDays(7);
                else if (numberOfTimesRateAppShown < 3)
                    minimalDateToShowRateAppPrompt = minimalDateToShowRateAppPrompt.AddDays(14);
                else
                    minimalDateToShowRateAppPrompt = minimalDateToShowRateAppPrompt.AddDays(30);
            
                _storageService.Store(MinimalDateToShowRateAppPromptKey, minimalDateToShowRateAppPrompt);
            }

            return true;
        }

        public void RateApp()
        {
            _storageService.Store(IsAppRatedPromptKey, true);
            LaunchAppRateProcess();
        }

        public void DisableShowingAppRatePrompt()
        {
            _storageService.Store(IsRateAppPromptEnabledKey, false);
        }

        protected bool IsAppRatePromptEnabled => _storageService.Get(IsRateAppPromptEnabledKey, true);

        public abstract void LaunchAppRateProcess();
    }
}