using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Services;
using Debts.ViewModel;

namespace Debts.Commands.FinanceDetails
{
    public class TransferToFinanceDetailsCommandBuilder : AsyncGuardedCommandBuilder<FinanceOperation>
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;
        
        public TransferToFinanceDetailsCommandBuilder( QueryCommandExecutor queryCommandExecutor)
        {
            _queryCommandExecutor = queryCommandExecutor;
        }

        protected override bool ShouldNotifyAboutProgress => false;

        protected override async Task ExecuteCommandAction(FinanceOperation item)
        {
            var operation = await _queryCommandExecutor.Execute(new GetFinanceDetailsWithChildrensQuery(item));

            EnqueueAfterCommandExecuted(() =>
            {
                ServicesLocation.NavigationService.Navigate<FinanceDetailsViewModel, FinanceOperation>(operation);
            });
        }
    }
}