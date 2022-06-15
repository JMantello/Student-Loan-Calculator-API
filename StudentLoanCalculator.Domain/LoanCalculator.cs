namespace StudentLoanCalculator.Domain
{
    public class LoanCalculator : ILoanCalculator
    {
        public LoanCalculator()
        {
        }

        public double GetInterest(int years, double rate)
        {
            // Calculate interest after years
            return 0;
        }

        public double GetMonthlyPayment(double loanCost, int yearsRemaining, double interestRate)
        {
            if (interestRate == null)
            {
                interestRate = 0.058;
            }

            // Get Monthly Interest Rate
            interestRate = interestRate / 12D;

            // Get Months Remaining
            int monthsRemaining = yearsRemaining * 12;

            double monthlyPayment = loanCost * interestRate * (Math.Pow((1 + interestRate), monthsRemaining) / (Math.Pow((1 + interestRate), monthsRemaining) - 1));

            return monthlyPayment;
        }
    }
}