namespace StudentLoanCalculator.Api.Models
{
    public class CalculationOutputModel
    {
        public double MonthlyPaymentToLoan { get; set; }
        public double MonthlyPaymentToInvest{ get; set; }
        public double InterestPaid { get; set; }
        public double InvestmentValue { get; set; }
        public double ReturnOnInvestment { get; set; }
        public double SuggestedInvestmentAmount { get; set; }
        public List<double>? RemainingLoanBalances { get; set; }

        // Net Worth
        public int NetWorth { get; set; }

    }
}
