using System.Threading.Tasks;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Messenging.Messages.App;
using Debts.Services;
using Debts.ViewModel;

namespace Debts.Commands.FinanceDetails
{
    public class ToggleFavouriteStateCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly FinanceDetailsViewModel _financeDetailsViewModel;
        private readonly QueryCommandExecutor _queryCommandExecutor;

        public ToggleFavouriteStateCommandBuilder(FinanceDetailsViewModel financeDetailsViewModel, QueryCommandExecutor queryCommandExecutor)
        {
            _financeDetailsViewModel = financeDetailsViewModel;
            _queryCommandExecutor = queryCommandExecutor;
        }

        protected override async Task ExecuteCommandAction()
        {
            bool isFavourite = await _queryCommandExecutor.Execute(new ToggleFavouriteStateFinanceOperationDatabaseCommand(_financeDetailsViewModel.Details));

            _financeDetailsViewModel.Details.IsFavourite = isFavourite;
            _financeDetailsViewModel.RaisePropertyChanged(() => _financeDetailsViewModel.IsFavourite);
            
            ServicesLocation.Messenger.Publish(new FinanceFavoriteStateChangedMvxMessage(this, _financeDetailsViewModel.Details));
        }
    }
}