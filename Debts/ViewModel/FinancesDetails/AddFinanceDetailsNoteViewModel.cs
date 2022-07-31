using System;
using Debts.Commands;
using Debts.Commands.FinanceDetails;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.Model.NavigationData;
using Debts.Services;
using MvvmCross.Commands;

namespace Debts.ViewModel.FinancesDetails
{
    public class AddFinanceDetailsNoteViewModel : BaseViewModel<AddFinanceOperationNoteNavigationData>
    {
        private readonly QueryCommandExecutor _commandExecutor;

        public AddFinanceDetailsNoteViewModel(QueryCommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }
        
        public override void Prepare(AddFinanceOperationNoteNavigationData parameter)
        {
            base.Prepare(parameter);
            Type = parameter.Type;
            Operation = parameter.Operation;
            Duration = parameter.Duration;
        }
        
        public TimeSpan? Duration { get; set; }
        
        public FinanceOperation Operation { get; set; }
        
        public NoteType Type { get; set; }
        
        public string Note { get; set; }
        
        public MvxCommand Close => new MvxExceptionGuardedCommand(() => ServicesLocation.NavigationService.Close(this));

        public MvxCommand AddNote => new AddFinanceNoteCommandBuilder(_commandExecutor, this).BuildCommand();
    }
}