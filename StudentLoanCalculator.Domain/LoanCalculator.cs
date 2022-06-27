namespace StudentLoanCalculator.Domain
{
    public class LoanCalculator : ILoanCalculator
    {
        public LoanCalculator()
        {
        }

        public double GetMonthlyLoanPayment(double loanAmount, double interestRate, int timeInMonths, double minimumPayment)
        {
            double monthlyPayment = loanAmount * interestRate * 
                (Math.Pow((1 + interestRate), timeInMonths) / (Math.Pow((1 + interestRate), timeInMonths) - 1));

            return monthlyPayment;
        }

        public double GetMonthlyInvestmentPayment(double loanPayment, double discretionaryIncome)
        {
            return discretionaryIncome - loanPayment;
        }

        public double GetLoanInterest(double loanAmount, int timeInMonths, double monthlyPayment)
        {
            double totalPaid = monthlyPayment * timeInMonths;
            double interestPaid = totalPaid - loanAmount;
            return interestPaid;
        }

        public double GetProjectedInvestment(double monthlyInvestment, double growthRate, int timeInMonths)
        {
            double investmentTotal = monthlyInvestment * ((Math.Pow((1 + growthRate), timeInMonths) - 1) / growthRate); 
            return investmentTotal;
        }
        
        public double GetReturnOnInvestment(double monthlyInvestment, int timeInMonths, double investmentTotal)
        {
            double returnOnInvestment = investmentTotal - (monthlyInvestment * timeInMonths);
            return returnOnInvestment;
        }

        public double GetSuggestedInvestment(double loanInterest, double growthRate, double timeInMonths)
        {
            double suggestedInvestment = - (growthRate * loanInterest) / 
                (- Math.Pow(growthRate + 1, timeInMonths) + ((timeInMonths * growthRate) + 1));
            return suggestedInvestment;
        }

        public List<double> GetRemainingLoanBalances(double loanAmount, double interestRate, double monthlyPayment, int timeInMonths)
        {
            List<double> remainingLoanBalances = new List<double>();
            double currentLoanAmount = loanAmount;
            
            for(int i = timeInMonths; i > 0; i--)
            {
                double loanBalance = (currentLoanAmount * (1 + interestRate)) - monthlyPayment;

                if(loanBalance <= 0)
                {
                    remainingLoanBalances.Add(0);
                    currentLoanAmount = 0;
                }
                else
                {
                    remainingLoanBalances.Add(loanBalance);
                    currentLoanAmount = loanBalance;
                }
            }

            return remainingLoanBalances;
        }

        public double NetWorthCalculator(double cashAsset, double propertyAsset, double investmentsAsset, double mortgageLiability, double loansLiabilty, double debtsLiability, int[] time, double monthlyINvestment, double monthlyLoanPayment, double studentLoanCost, double inflationRate)
        {
            throw new NotImplementedException();
        }
    }
}