using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentLoanCalculator.Domain
{
    public interface ILoanCalculator
    {
        public double GetMonthlyLoanPayment(double loanAmount, double interestRate, int timeInMonths, double minimumPayment);
        public double GetMonthlyInvestmentPayment(double loanPayment, double discretionaryIncome);
        public double GetLoanInterest(double loanAmount, int timeInMonths, double monthlyPayment);
        public double GetProjectedInvestment(double monthlyInvestment, double growthRate, int timeInMonths);
        public double GetReturnOnInvestment(double monthlyInvestment, int timeInMonths, double investmentTotal);
        public double GetSuggestedInvestment(double loanInterest, double growthRate, double timeInMonths);
        public List<double> GetRemainingLoanBalances(double loanAmount, double interestRate, double monthlyPayment, int timeInMonths);

        // Net Worth Calculation
        List<double> GetProjectedNetWorth(double cashAsset, double propertyAsset, double investmentsAsset, double mortgageLiability, double loansLiabilty, double debtsLiability, int timeInYears, double monthlyInvestment, double monthlyLoanPayment, double studentLoanCost, double loanInterestRate, double inflationRate, double investmentGrowthRate);
    }
}
