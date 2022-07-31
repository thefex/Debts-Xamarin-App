using System;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Services.Payments;

namespace Debts.Services.AppGrowth
{
    public class PremiumService
    {
        private readonly AppRelatedStatisticsService _appRelatedStatisticsService;
        private readonly IBillingService _billingService;

        public PremiumService(AppRelatedStatisticsService appRelatedStatisticsService, IBillingService billingService)
        {
            _appRelatedStatisticsService = appRelatedStatisticsService;
            _billingService = billingService; 
        }

        
        public async Task Initialize()
        {
            await _billingService.Initialize();
        }

        public bool HasPremium => PremiumState != PremiumState.Limited;

        public bool AreAdsAvailable => 
            PremiumState == PremiumState.OneDayExtendedTrial || PremiumState == PremiumState.Limited; 

        public bool ShouldShowGoPremiumScreen =>
            !HasPremium && _appRelatedStatisticsService.TimePassedSinceInstallation > TimeSpan.FromDays(14);

        public PremiumState PremiumState
        {
            get
            {
                if (_billingService.HasPremiumLifeTime)
                    return PremiumState.PremiumOwnership;

                if (_billingService.HasPremiumSubscription)
                    return PremiumState.PremiumSubscription;
                
                if (_appRelatedStatisticsService.IsExtendedTrialValid())
                    return PremiumState.OneDayExtendedTrial;

                if (_appRelatedStatisticsService.TimePassedSinceInstallation > TimeSpan.FromDays(7))
                    return PremiumState.Limited;
                    
                return PremiumState.Trial;
            }
        }

        public int GetAmountOfDaysOfValidPremiumSubscription()
        {
            if (PremiumState == PremiumState.PremiumOwnership)
                return 0;
            
            if (PremiumState == PremiumState.Trial)
            { 
                return 7-(int)(_appRelatedStatisticsService.TimePassedSinceInstallation.TotalDays);
            }

            if (PremiumState == PremiumState.OneDayExtendedTrial)
                return 1;

            if (PremiumState == PremiumState.PremiumSubscription)
                return 30;

            return 0;
        }

        public bool CanExtendPremiumByOneDay() => PremiumState == PremiumState.Limited && GetAmountOfDaysOfValidPremiumSubscription() == 0;

        public void ExtendTrialByOneDay()
        {
            _appRelatedStatisticsService.ExtendTrialByOneDay();
        }

        public Task BuyMonthlySubscription() => _billingService.BuyMonthlySubscription();

        public Task BuyLifetimePremium() => _billingService.BuyLifetimeSubscription();

        public Task UpdateState() => _billingService.UpdateState();

        public bool CanMakePayments => _billingService.CanMakePayments;

        public event Action PremiumStateUpdated
        {
            add => _billingService.PremiumStateUpdated += value;
            remove => _billingService.PremiumStateUpdated -= value;
        }

        public void RestorePurchase() => _billingService.Restore();
    }
}