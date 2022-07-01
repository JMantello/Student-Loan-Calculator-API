namespace StudentLoanCalculator.Api.Models
{
    public class OutputModel
    {
        public double MonthlyPaymentToLoan { get; set; }
        public double MonthlyPaymentToInvest{ get; set; }
        public double InterestPaid { get; set; }
        public double InvestmentValue { get; set; }
        public double ReturnOnInvestment { get; set; }
        public double SuggestedInvestmentAmount { get; set; }
        public List<double>? RemainingLoanBalances { get; set; }
        public List<double>? NetWorth { get; set; }


    }
}
