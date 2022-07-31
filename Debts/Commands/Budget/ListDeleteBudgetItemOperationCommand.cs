using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Budget;
using Debts.Messenging.Messages;
using Debts.Resources;
using Debts.Services;
using DynamicData;

namespace Debts.Commands.Budget
{
    public class ListDeleteBudgetItemOperationCommand : AsyncGuardedCommandBuilder<BudgetItem>
    {
        private readonly ISourceList<BudgetItem> _source;
        private readonly QueryCommandExecutor _queryCommandExecutor;

        public ListDeleteBudgetItemOperationCommand(ISourceList<BudgetItem> source, QueryCommandExecutor queryCommandExecutor)
        {
            _source = source;
            _queryCommandExecutor = queryCommandExecutor;
        }
        
        protected override bool ShouldNotifyAboutProgress => false;
        protected override Task ExecuteCommandAction(BudgetItem item)
        {
            _source.Remove(item);
            
            ServicesLocation.Messenger.Publish(new ToastMvxMessage(this, TextResources.Command_ListDeleteBudget_ToastContent)
            {
                Style = ToastMvxMessage.ToastStyle.Info,
                ActionText = TextResources.Toast_ActionText_Undo,
                ActionCommand = new MvxExceptionGuardedCommand(() =>
                {
                    _source.Add(item);
                }),
                DismissCommand = new MvxExceptionGuardedCommand(() =>
                    {
                        _queryCommandExecutor.Execute(new DeleteBudgetItemCommand(item));
                    })
            });

            return Task.FromResult(true);
        }
    }
}