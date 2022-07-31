using Debts.Model.Sections;

namespace Debts.iOS.Core.Data
{
    public class EmptyFinanceOperationSection
    {
        public DetailsFinanceOperationsSection OperationsSection { get; }

        public EmptyFinanceOperationSection(DetailsFinanceOperationsSection operationsSection)
        {
            OperationsSection = operationsSection;
        }
    }
}