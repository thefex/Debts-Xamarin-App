using Debts.Model.Sections;

namespace Debts.iOS.Core.Data
{
    public class HeaderFinanceOperationSection
    {
        public DetailsFinanceOperationsSection OperationsSection { get; }
        
        public HeaderFinanceOperationSection(DetailsFinanceOperationsSection operationsSection)
        {
            OperationsSection = operationsSection;
        }
    }
}