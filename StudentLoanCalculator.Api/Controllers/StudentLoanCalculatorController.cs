using Microsoft.AspNetCore.Mvc;
using StudentLoanCalculator.Api.Models;
using StudentLoanCalculator.Domain;

namespace StudentLoanCalculator___Team_1.Controllers
{
    [Route("[controller]")]
    public class StudentLoanCalculatorController : Controller
    {
        private ILoanCalculator calc;

        public StudentLoanCalculatorController(ILoanCalculator loanCalculator)
        {
            this.calc = loanCalculator;
        }

        public IActionResult Index()
        {
            return Json("From StudentLoanCalculator");
        }

        [HttpGet("Calculate")] 
        public CalculationOutputModel Calculate(CalculationInputModel input)
        {
            CalculationOutputModel outputModel = new CalculationOutputModel();

            // Validation
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid");
                return outputModel;
            }

            if (input.DiscretionaryIncome < input.MinimumPayment)
            {
                Console.WriteLine("Discretionary income must exceed minimum payment");
                return outputModel;
            }

            if (input.TermInYears <= 0)
            {
                Console.WriteLine("Loan period must be greater than 0");
                return outputModel;
            }

            // Conversions / Defining Values
            double discretionaryIncome = input.DiscretionaryIncome;
            double loanAmount = input.LoanAmount;
            double term = input.TermInYears; // Rename
            double minimumPayment = input.MinimumPayment;
            
            // Convert from percentage to decimal point
            double interestRate;
            if (input.InterestRate >= 1) { interestRate = input.InterestRate * 0.01D; }
            else { interestRate = input.InterestRate; }

            // Convert from percentage to decimal point
            double investmentGrowthRate;
            if (input.InvestmentGrowthRate >= 1) { investmentGrowthRate = input.InvestmentGrowthRate/ 100; }
            else { investmentGrowthRate = input.InvestmentGrowthRate; }

            double monthlyInterestRate = interestRate / 12;
            int termInMonths = input.TermInYears * 12;
            double monthlyInvestmentGrowthRate = investmentGrowthRate / 12;

            // Calculations
            double monthlyLoanPayment = calc.GetMonthlyLoanPayment(loanAmount, monthlyInterestRate, termInMonths, minimumPayment);
            double monthlyInvestmentPayment = calc.GetMonthlyInvestmentPayment(monthlyLoanPayment, discretionaryIncome);
            double loanInterest = calc.GetLoanInterest(loanAmount, termInMonths, monthlyLoanPayment);
            double projectedInvestmentTotal = calc.GetProjectedInvestment(monthlyInvestmentPayment, monthlyInvestmentGrowthRate, termInMonths);
            double returnOnInvestment = calc.GetReturnOnInvestment(monthlyInvestmentPayment, termInMonths, projectedInvestmentTotal);
            double suggestedInvestmentAmount = calc.GetSuggestedInvestment(loanInterest, monthlyInvestmentGrowthRate, termInMonths);
            List<double> remainingLoanBalances = calc.GetRemainingLoanBalances(loanAmount, monthlyInterestRate, monthlyLoanPayment, termInMonths);

            outputModel.MonthlyPaymentToLoan = monthlyLoanPayment;
            outputModel.MonthlyPaymentToInvest = monthlyInvestmentPayment;
            outputModel.InterestPaid = loanInterest;
            outputModel.InvestmentValue = projectedInvestmentTotal;
            outputModel.ReturnOnInvestment = returnOnInvestment;
            outputModel.SuggestedInvestmentAmount = suggestedInvestmentAmount;
            outputModel.RemainingLoanBalances = remainingLoanBalances;

            return outputModel;
        }
    }
}
