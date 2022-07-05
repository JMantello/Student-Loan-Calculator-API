namespace StudentLoanCalculator.Api.Models
{
    public class InputModel
    {
        // Loan Data
        public double DiscretionaryIncome { get; set; } // Monthly
        public double LoanAmount { get; set; }
        public double InterestRate { get; set; }
        public int TermInYears { get; set; } 
        public double MinimumPayment { get; set; }
        
        // Investment Risk
        public double InvestmentGrowthRate { get; set; } 

    }
}
