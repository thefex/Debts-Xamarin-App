using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Messenging.Messages;
using Debts.Resources;
using Debts.Services;
using DynamicData;

namespace Debts.Commands.Finances
{
    public class ListDeleteFinanceOperationCommand : AsyncGuardedCommandBuilder<FinanceOperation>
    {
        private readonly ISourceList<FinanceOperation> _source;
        private readonly QueryCommandExecutor _queryCommandExecutor;

        public ListDeleteFinanceOperationCommand(ISourceList<FinanceOperation> source, QueryCommandExecutor queryCommandExecutor)
        {
            _source = source;
            _queryCommandExecutor = queryCommandExecutor;
        }
        
        protected override bool ShouldNotifyAboutProgress => false;
        protected override Task ExecuteCommandAction(FinanceOperation item)
        {
            _source.Remove(item);
            
            ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, TextResources.Command_ListDeleteFinanceOperation_ToastContent)
            {
                Style = ToastMvxMessage.ToastStyle.Info,
                ActionText = TextResources.Toast_ActionText_Undo,
                ActionCommand = new MvxExceptionGuardedCommand(() =>
                {
                    _source.Add(item);
                }),
                DismissCommand = new MvxExceptionGuardedCommand(() =>
                    {
                        _queryCommandExecutor.Execute(new DeleteFinanceOperationDatabaseCommand(item));
                    })
            });

            return Task.FromResult(true);
        }
    }
}