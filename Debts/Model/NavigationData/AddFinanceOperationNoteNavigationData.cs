using System;
using Debts.Data;
using Debts.ViewModel;

namespace Debts.Model.NavigationData
{
    public class AddFinanceOperationNoteNavigationData
    {
        public FinanceOperation Operation { get; }
        public TimeSpan? Duration { get; }
        public NoteType Type { get; }
        

        public AddFinanceOperationNoteNavigationData(FinanceOperation operation, TimeSpan? duration, NoteType type)
        {
            Operation = operation;
            Duration = duration;
            Type = type;
        }
    }
}