using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Messenging;
using Debts.Messenging.Messages.App;
using Debts.Services.AppGrowth;
using DynamicData;

namespace Debts.ViewModel.FinancesViewModel
{
    public class FavoritesFinancesViewModel : BaseFinancesViewModel
    {
        public FavoritesFinancesViewModel(QueryCommandExecutor queryCommandExecutor, PremiumService premiumService, AdvertisementService advertisementService) : base(queryCommandExecutor, premiumService, advertisementService)
        {
            MessageObserversController.AddObservers(
                new InvokeActionMessageObserver<FinanceFavoriteStateChangedMvxMessage>(
                    msg =>
                    {
                        if (!msg.FinanceOperation.IsFavourite)
                            ItemsSource.Remove(msg.FinanceOperation);
                        else 
                            ItemsSource.Add(msg.FinanceOperation);
                    }));
        }

        protected override IListDataQuery<FinanceOperation> BuildQuery(FinanceOperationsQueryParameter limitOffsetQueryParameter) => new GetFavoritesFinancesQuery(limitOffsetQueryParameter);
    }
}