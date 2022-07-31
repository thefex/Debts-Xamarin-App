using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Services.AppGrowth;

namespace Debts.ViewModel.FinancesViewModel
{
    public class MyDebtsViewModel : BaseFinancesViewModel
    {
        public MyDebtsViewModel(QueryCommandExecutor queryCommandExecutor, PremiumService premiumService, AdvertisementService advertisementService) : base(queryCommandExecutor, premiumService, advertisementService)
        {
        }

        protected override IListDataQuery<FinanceOperation> BuildQuery(FinanceOperationsQueryParameter limitOffsetQueryParameter)
            => new GetMyDebtsQuery(limitOffsetQueryParameter);
    }
}