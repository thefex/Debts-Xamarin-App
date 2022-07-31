namespace Debts.Model.Sections
{
    public class DetailsStatisticsSection : DetailsSection
    {
        public decimal TotalLoanAmount { get; set; } = 1250;

        public decimal TotalDebtAmount { get; set; } = 640;

        public decimal PaidLoanAmount { get; set; } = 0;

        public decimal PaidDebtAmount { get; set; } = 125;

        public decimal RemainingLoanAmount => TotalLoanAmount - PaidLoanAmount;

        public decimal RemainingDebtAmount => TotalDebtAmount - PaidDebtAmount;
    }
}