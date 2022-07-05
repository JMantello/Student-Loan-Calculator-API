using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using StudentLoanCalculator.Api.Data;
using StudentLoanCalculator.Api.Identity;
using StudentLoanCalculator.Api.Models;
using StudentLoanCalculator.Domain;

namespace StudentLoanCalculator___Team_1.Controllers
{
    [Route("[controller]")]
    public class StudentLoanCalculatorController : Controller
    {
        //private MongoCRUD context;
        private ILoanCalculator calc;
        private GrowthRatesModel growthRates;

        public StudentLoanCalculatorController(ILoanCalculator loanCalculator) //, MongoCRUD context
        {
            //this.context = context;
            //context.LoadRecords<GrowthRatesModel>("GrowthRates")[0];

            this.calc = loanCalculator;

            growthRates = new GrowthRatesModel
            {
                inflation = 0.086,
                conservative = 0.149,
                moderate = 0.173,
                aggressive = 0.238
            };
        }

        public IActionResult Index()
        {
            return Json("Student Loan Calculator - Team 1");
        }

        [HttpGet("Calculate")]
        public OutputModel Calculate(InputModel input)
        {
            OutputModel outputModel = new OutputModel();

            // Validate input
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
            
            // Other validations?

            // Destructure input
            double discretionaryIncome = input.DiscretionaryIncome;
            double loanAmount = input.LoanAmount;
            int termInYears = input.TermInYears;
            int termInMonths = termInYears * 12;
            double minimumPayment = input.MinimumPayment;
            double inflationRate = growthRates.inflation;
            double interestRate = input.InterestRate * 0.01D; // Convert from percentage to decimal
            double monthlyInterestRate = interestRate / 12;
            double investmentGrowthRate = input.InvestmentGrowthRate * 0.01D;
            double monthlyInvestmentGrowthRate = investmentGrowthRate / 12;

            // Calculations
            double monthlyPaymentToLoan = MonthlyLoanPayment(loanAmount, monthlyInterestRate, termInMonths, minimumPayment);
            
            double monthlyPaymentToInvest = MonthlyInvestmentPayment(monthlyPaymentToLoan, discretionaryIncome);
            
            double interestPaid = LoanInterest(loanAmount, termInMonths, monthlyPaymentToLoan);
            
            double projectedInvestment = ProjectedInvestment(monthlyPaymentToInvest, monthlyInvestmentGrowthRate, termInMonths);
            
            double returnOnInvestment = ReturnOnInvestment(monthlyPaymentToInvest, termInMonths, projectedInvestment);
            
            double suggestedInvestmentAmount = SuggestedInvestment(interestPaid, monthlyInvestmentGrowthRate, termInMonths);
            
            List<double> remainingLoanBalances = RemainingLoanBalances(loanAmount, monthlyInterestRate, monthlyPaymentToLoan, termInMonths);
           
            List<double> monthlyInvestmentGrowth = MonthlyInvestmentGrowth(monthlyPaymentToInvest, monthlyInvestmentGrowthRate, termInMonths);

            List<double> monthlyNetWorthImpact = MonthlyNetWorthImpact(remainingLoanBalances.ToArray(), monthlyInvestmentGrowth.ToArray());

            // Bind output
            outputModel.MonthlyPaymentToLoan = monthlyPaymentToLoan;
            outputModel.MonthlyPaymentToInvest = monthlyPaymentToInvest;
            outputModel.InterestPaid = interestPaid;
            outputModel.TotalPaidToLoan = loanAmount + interestPaid;
            outputModel.ProjectedInvestment = projectedInvestment;
            outputModel.ReturnOnInvestment = returnOnInvestment;
            outputModel.SuggestedInvestmentAmount = suggestedInvestmentAmount;
            outputModel.RemainingLoanBalances = remainingLoanBalances;
            outputModel.MonthlyInvestmentGrowth = monthlyInvestmentGrowth;
            outputModel.MonthlyNetWorthImpact = monthlyNetWorthImpact;

            return outputModel;
        }

        [HttpGet("MonthlyLoanPayment")]
        public double MonthlyLoanPayment(double loanAmount, double monthlyInterestRate, int termInMonths, double minimumPayment)
        {
            double monthlyLoanPayment = Math.Round(calc.MonthlyLoanPayment(loanAmount, monthlyInterestRate, termInMonths, minimumPayment), 2, MidpointRounding.AwayFromZero);

            return monthlyLoanPayment;
        }

        [HttpGet("MonthlyInvestmentPayment")]
        public double MonthlyInvestmentPayment(double monthlyLoanPayment, double discretionaryIncome)
        {
            double monthlyInvestmentPayment = Math.Round(calc.MonthlyInvestmentPayment(monthlyLoanPayment, discretionaryIncome), 2);

            return monthlyInvestmentPayment;
        }

        [HttpGet("LoanInterest")]
        public double LoanInterest(double loanAmount, int termInMonths, double monthlyLoanPayment)
        {
            double loanInterest = Math.Round(calc.LoanInterest(loanAmount, termInMonths, monthlyLoanPayment), 2);

            return loanInterest;
        }

        [HttpGet("ProjectedInvestment")]
        public double ProjectedInvestment(double monthlyInvestmentPayment, double monthlyInvestmentGrowthRate, int termInMonths)
        {
            double projectedInvestmentTotal = Math.Round(calc.ProjectedInvestment(monthlyInvestmentPayment, monthlyInvestmentGrowthRate, termInMonths), 2);

            return projectedInvestmentTotal;
        }

        [HttpGet("ReturnOnInvestment")]
        public double ReturnOnInvestment(double monthlyInvestmentPayment, int termInMonths, double projectedInvestmentTotal)
        {
            double returnOnInvestment = Math.Round(calc.ReturnOnInvestment(monthlyInvestmentPayment, termInMonths, projectedInvestmentTotal), 2);

            return returnOnInvestment;
        }

        [HttpGet("SuggestedInvestment")]
        public double SuggestedInvestment(double loanInterest, double monthlyInvestmentGrowthRate, double termInMonths)
        {
            // Finds the monthly amount to invest so that at the end of the term, the calculated growth will be equal to the interest cost from the loan.
            double suggestedInvestmentAmount = Math.Round(calc.SuggestedInvestment(loanInterest, monthlyInvestmentGrowthRate, termInMonths), 2);

            return suggestedInvestmentAmount;
        }

        [HttpGet("RemainingLoanBalances")]
        public List<double> RemainingLoanBalances(double loanAmount, double monthlyInterestRate, double monthlyLoanPayment, int termInMonths)
        {
            List<double> remainingLoanBalances = calc.RemainingLoanBalances(loanAmount, monthlyInterestRate, monthlyLoanPayment, termInMonths);

            return remainingLoanBalances;
        }

        [HttpGet("MonthlyInvestmentGrowth")]
        public List<double> MonthlyInvestmentGrowth(double monthlyInvestment, double monthlyInvestmentGrowthRate, int months)
        {
            List<double> monthlyInvestmentGrowthValues = calc.MonthlyInvestmentGrowth(monthlyInvestment, monthlyInvestmentGrowthRate, months);

            return monthlyInvestmentGrowthValues;
        }

        [HttpGet("MonthlyNetWorthImpact")]
        public List<double> MonthlyNetWorthImpact(double[] liabilityRemaining, double[] assets)
        {
            List<double> monthlyNetWorthImpact = calc.MonthlyNetWorthImpact(liabilityRemaining, assets);

            return monthlyNetWorthImpact;
        }
    }
}
