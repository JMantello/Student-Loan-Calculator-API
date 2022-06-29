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
        private ILoanCalculator calc;
        private MongoCRUD context;
        private Rates rates;

        public StudentLoanCalculatorController(ILoanCalculator loanCalculator, MongoCRUD context)
        {
            this.calc = loanCalculator;
            this.context = context;
            //rates = context.LoadRecords<Rates>("Rates")[0];
        }

        public IActionResult Index()
        {
            return Json("From Student Loan Calculator");
        }

        [HttpGet("Calculate")] 
        public OutputModel Calculate(InputModel input)
        {
            OutputModel outputModel = new OutputModel();

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
            int termInYears = input.TermInYears;
            double minimumPayment = input.MinimumPayment;
            double inflationRate = 0.086; // Call db
            
            // Convert from percentage to decimal point
            double interestRate;
            if (input.InterestRate >= 1) { interestRate = input.InterestRate * 0.01D; }
            else { interestRate = input.InterestRate; }

            // Convert from percentage to decimal point
            double investmentGrowthRate;
            if (input.InvestmentGrowthRate >= 1) { investmentGrowthRate = input.InvestmentGrowthRate/ 100; }
            else { investmentGrowthRate = input.InvestmentGrowthRate; }

            double monthlyInterestRate = interestRate / 12;
            int termInMonths = termInYears * 12;
            double monthlyInvestmentGrowthRate = investmentGrowthRate / 12;

            // Calculate student loan
            double monthlyLoanPayment = Math.Round(calc.GetMonthlyLoanPayment(loanAmount, monthlyInterestRate, termInMonths, minimumPayment), 2, MidpointRounding.AwayFromZero);
            double monthlyInvestmentPayment = Math.Round(calc.GetMonthlyInvestmentPayment(monthlyLoanPayment, discretionaryIncome), 2);
            double loanInterest = Math.Round(calc.GetLoanInterest(loanAmount, termInMonths, monthlyLoanPayment), 2);
            double projectedInvestmentTotal = Math.Round(calc.GetProjectedInvestment(monthlyInvestmentPayment, monthlyInvestmentGrowthRate, termInMonths), 2);
            double returnOnInvestment = Math.Round(calc.GetReturnOnInvestment(monthlyInvestmentPayment, termInMonths, projectedInvestmentTotal), 2);
            double suggestedInvestmentAmount = Math.Round(calc.GetSuggestedInvestment(loanInterest, monthlyInvestmentGrowthRate, termInMonths), 2);
            List<double> remainingLoanBalances = calc.GetRemainingLoanBalances(loanAmount, monthlyInterestRate, monthlyLoanPayment, termInMonths);

            // Calculate net worth
            double cashAsset = input.CashAsset;
            double propertyAsset = input.PropertyAsset;
            double investmentAsset = input.InvestmentsAsset;
            double mortgageLiability = input.MortgageLiability;
            double otherLoansLiability = input.OtherLoansLiability;
            double debtsLiability = input.OtherDebtsLiability;

            List<double> netWorth = calc.GetProjectedNetWorth(cashAsset, propertyAsset, investmentAsset, mortgageLiability, otherLoansLiability, debtsLiability, termInYears, monthlyInvestmentPayment, monthlyLoanPayment, loanAmount, interestRate, inflationRate, investmentGrowthRate);

            outputModel.MonthlyPaymentToLoan = monthlyLoanPayment;
            outputModel.MonthlyPaymentToInvest = monthlyInvestmentPayment;
            outputModel.InterestPaid = loanInterest;
            outputModel.InvestmentValue = projectedInvestmentTotal;
            outputModel.ReturnOnInvestment = returnOnInvestment;
            outputModel.SuggestedInvestmentAmount = suggestedInvestmentAmount;
            outputModel.RemainingLoanBalances = remainingLoanBalances;
            outputModel.NetWorth = netWorth;

            return outputModel;
        }
    }
}
