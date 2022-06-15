using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentLoanCalculator.Domain
{
    public interface ILoanCalculator
    {
        public double GetInterest(int years, double rate);
        public double GetMonthlyPayment(double loanCost, int yearsRemaining, double interestRate);


    }
}
