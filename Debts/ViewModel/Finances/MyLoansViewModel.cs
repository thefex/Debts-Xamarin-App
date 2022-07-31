using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Services.AppGrowth;
using Debts.ViewModel.FinancesViewModel;

namespace Debts.ViewModel.Finances
{
    public class MyLoansViewModel : BaseFinancesViewModel
    {
        public MyLoansViewModel(QueryCommandExecutor queryCommandExecutor, PremiumService premiumService, AdvertisementService advertisementService) : base(queryCommandExecutor, premiumService, advertisementService)
        {
        }

        protected override IListDataQuery<FinanceOperation> BuildQuery(FinanceOperationsQueryParameter limitOffsetQueryParameter)
            => new GetMyLoansQuery(limitOffsetQueryParameter);
    }
}