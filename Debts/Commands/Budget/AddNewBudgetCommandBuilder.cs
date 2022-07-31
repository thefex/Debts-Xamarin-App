using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Budget;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Extensions;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Resources;
using Debts.Services;
using Debts.Services.LocationService;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Finances;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Debts.Commands.Budget
{
    public class AddNewBudgetCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly AddBudgetViewModel _addBudgetViewModel;
        private readonly QueryCommandExecutor _queryCommandExecutor;

        public AddNewBudgetCommandBuilder(
            AddBudgetViewModel addBudgetViewModel, QueryCommandExecutor queryCommandExecutor)
        {
            _addBudgetViewModel = addBudgetViewModel;
            _queryCommandExecutor = queryCommandExecutor;
        }
        
        protected override async Task ExecuteCommandAction()
        {
            var insertedBudget = await _queryCommandExecutor.Execute(new InsertBudgetItemCommand(new BudgetItem()
            {
                Title = _addBudgetViewModel.Title,
                Amount = _addBudgetViewModel.Amount.Value,
                Category = _addBudgetViewModel.SelectedCategory,
                CategoryId = _addBudgetViewModel.SelectedCategory.MainCategoryId,
                CreatedAt = DateTime.UtcNow,
                Currency = _addBudgetViewModel.Currency,
                Type = _addBudgetViewModel.Type
            }));
            
            EnqueueAfterCommandExecuted(() =>
              {
                  ServicesLocation.Messenger.Publish(new ItemPublishedMessage<BudgetItem>(this, insertedBudget));
                  _addBudgetViewModel.Close.Execute();
              });
        } 
    }
}