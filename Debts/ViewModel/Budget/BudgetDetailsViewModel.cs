using System.Collections.Generic;
using Debts.Commands;
using Debts.Commands.Budget;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.Model.Sections;
using Debts.Services;
using MvvmCross.Commands;

namespace Debts.ViewModel.Budget
{
    public class BudgetDetailsViewModel : BaseViewModel<BudgetItem>
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;

        public BudgetDetailsViewModel(QueryCommandExecutor queryCommandExecutor)
        {
            _queryCommandExecutor = queryCommandExecutor;
        }
        
        public override void Prepare(BudgetItem parameter)
        {
            base.Prepare(parameter);
            Item = parameter;
        }

        public IEnumerable<DetailsSection> Sections { get; } = new List<DetailsSection>();

        public MvxCommand Delete => new DeleteBudgetCommandBuilder(_queryCommandExecutor, this).BuildCommand();
        
        public MvxCommand Close => new MvxExceptionGuardedCommand(() => ServicesLocation.NavigationService.Close(this));
        
        public BudgetItem Item { get; private set; }
    }
}