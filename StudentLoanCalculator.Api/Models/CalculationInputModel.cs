namespace StudentLoanCalculator.Api.Models
{
    public class CalculationInputModel
    {
        // Loan Data
        public double DiscretionaryIncome { get; set; } // Monthly
        public double LoanAmount { get; set; }
        public double InterestRate { get; set; }
        public int TermInYears { get; set; } // Years
        public double MinimumPayment { get; set; }
        
        // Investment Risk
        public double InvestmentGrowthRate { get; set; } 

        // Net Worth
        // Assets
        public double CashAsset { get; set; }
        public double PropertyAsset { get; set; }
        public double InvestmentsAsset { get; set; }

        // Liabilities
        public double MortgageLiability { get; set; }
        public double LoansLiability { get; set; }
        public double DebtsLiability { get; set; }

    }
}
