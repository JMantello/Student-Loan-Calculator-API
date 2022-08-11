namespace StudentLoanCalculator.Api.Models
{
    public class InputModel
    {
        public record LoanInfoQuery( // ?
            double loanAmount, 
            double monthlyInterestRate, 
            double monthlyLoanPayment, 
            int termInMonths
        );

        public double MonthlyDiscretionaryIncome { get; set; } // Monthly
        public double LoanAmount { get; set; }
        public double InterestRate { get; set; }
        public int TermInYears { get; set; }
        public InvestmentRiskKind InvestmentRisk { get; set; } 
    }
}
