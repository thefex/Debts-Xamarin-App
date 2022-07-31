using System;
using System.Threading.Tasks;

namespace Debts.Services.Payments
{
    public interface IBillingService
    {
        Task Initialize();

        Task UpdateState();
        
        Task BuyLifetimeSubscription();

        Task BuyMonthlySubscription();
        
        bool HasPremiumSubscription { get; }
        
        bool HasPremiumLifeTime { get; }

        bool CanMakePayments { get; }

        void Restore();
        
        event Action PremiumStateUpdated;
    }
}