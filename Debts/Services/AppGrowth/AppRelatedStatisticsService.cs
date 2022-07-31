using System;
using System.Collections.Generic;

namespace Debts.Services.AppGrowth
{
    public abstract class AppRelatedStatisticsService
    {
        private const string AppRunCounterKey = "AppRunCounterKey";
        private const string ExtendedTrialKey = "ExtendedTrialKey";
        private readonly IStorageService _storageService;

        public AppRelatedStatisticsService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public TimeSpan TimePassedSinceInstallation =>
            DateTime.UtcNow - GetInstallationDate();
        
        public abstract DateTime GetInstallationDate();

        public void IncrementAppRunCounter()
        {
            _storageService.Store(AppRunCounterKey, GetAppRunCounter() + 1);
        }

        public int GetAppRunCounter()
        {
            return _storageService.Get<int>(AppRunCounterKey, 0);
        }

        public void ExtendTrialByOneDay()
        {
            _storageService.Store(ExtendedTrialKey, DateTime.UtcNow.AddDays(1));
        }

        public bool IsExtendedTrialValid()
        {
            return _storageService.Get<DateTime>(ExtendedTrialKey, DateTime.UtcNow) > DateTime.UtcNow.AddMinutes(1);
        }
    }
}