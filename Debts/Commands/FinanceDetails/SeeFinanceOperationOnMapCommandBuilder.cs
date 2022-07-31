using System.Threading.Tasks;
using Debts.Data;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.ViewModel.AppGrowth;
using Xamarin.Essentials;

namespace Debts.Commands.FinanceDetails
{
    public class SeeFinanceOperationOnMapCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly FinanceOperation _financeOperation;
        private readonly PremiumService _premiumService;

        public SeeFinanceOperationOnMapCommandBuilder(FinanceOperation financeOperation, PremiumService premiumService)
        {
            _financeOperation = financeOperation;
            _premiumService = premiumService;
        }

        protected override bool ShouldNotifyAboutProgress => false;

        protected override async Task ExecuteCommandAction()
        {
            if (!_premiumService.HasPremium)
            {
                ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
                return;
            }
            
            var options =  new MapLaunchOptions
            {
                Name = $"{_financeOperation.Title} ({_financeOperation.PaymentDetails.Currency} {_financeOperation.PaymentDetails.Amount})"
            };

            await Map.OpenAsync(_financeOperation.Latitude.Value, _financeOperation.Longitude.Value, options);
        }
    }
}