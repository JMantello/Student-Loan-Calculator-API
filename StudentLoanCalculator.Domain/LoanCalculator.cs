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
                (Math.Pow(1 + interestRate, timeInMonths) / (Math.Pow((1 + interestRate), timeInMonths) - 1));

            if (monthlyPayment >= minimumPayment)
            {
                return monthlyPayment;
            } else
            {
                return minimumPayment;
            }
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
                double loanBalance = Math.Round((currentLoanAmount * (1 + interestRate)) - monthlyPayment, 2);

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

        public List<double> GetProjectedNetWorth(double a_Cash, double a_Property, double a_Investments, double l_Mortgage, double l_Loans, double l_Debts, int timeInYears, double monthlyInvestment, double monthlyLoanPayment, double studentLoanCost, double loanInterestRate, double inflationRate, double investmentGrowthRate)
        {
            List<double> netWorthByYear = new List<double>();

            // Calculate Projected Values
            for(int year = 1; year <= timeInYears; year++)
            {
                double investments = (a_Investments + (monthlyInvestment * 12)) * Math.Pow(1 + investmentGrowthRate, year);
                double assetsOther = (a_Cash + a_Property) * Math.Pow(1 + 0.08, year);

                double liabilitiesOther = (l_Mortgage + l_Debts) * Math.Pow(1 + 0.6446, year);
                double loans = (l_Loans + studentLoanCost - monthlyLoanPayment * 12) * Math.Pow(1 + loanInterestRate, year);

                double netWorth = (assetsOther + investments - liabilitiesOther - loans) * (1 - inflationRate);
                netWorthByYear.Add(Math.Round(netWorth, 2));
            }

            return netWorthByYear;
        }
    }
}