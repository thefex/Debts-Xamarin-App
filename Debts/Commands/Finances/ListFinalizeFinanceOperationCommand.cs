using System.Threading.Tasks;
using Debts.Commands.FinanceDetails;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Messenging.Messages;
using Debts.Resources;
using Debts.Services;
using DynamicData;

namespace Debts.Commands.Finances
{
    public class ListFinalizeFinanceOperationCommand : AsyncGuardedCommandBuilder<FinanceOperation>
    {
        private readonly ISourceList<FinanceOperation> _source;
        private readonly QueryCommandExecutor _queryCommandExecutor;

        public ListFinalizeFinanceOperationCommand(ISourceList<FinanceOperation> source, QueryCommandExecutor queryCommandExecutor)
        {
            _source = source;
            _queryCommandExecutor = queryCommandExecutor;
        }
        
        protected override bool ShouldNotifyAboutProgress => false;
        protected override async Task ExecuteCommandAction(FinanceOperation item)
        {
            /*if (item.PaymentDetails.PaymentDate.HasValue)
            {
                ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, "This finance operation is already marked as paid.")
                {
                    Style = ToastMvxMessage.ToastStyle.Error
                });
                _source.Remove(item);
                _source.Add(item);
                return;
            } */

            _source.Remove(item);
            var newPaymentDetails = await _queryCommandExecutor.Execute(new FinalizeFinanceOperationDatabaseCommand(item));
            item.PaymentDetails = newPaymentDetails;
            _source.Add(item);
            
            ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, TextResources.Command_ListFinalizeFinanceOperation_ToastContent)
            {
                Style = ToastMvxMessage.ToastStyle.Info,
            });
        }
    }
}