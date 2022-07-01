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
            return Json("From Student Loan Calculator");
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

            // Destructure input
            double discretionaryIncome = input.DiscretionaryIncome;
            double loanAmount = input.LoanAmount;
            int termInYears = input.TermInYears;
            int termInMonths = termInYears * 12;
            double minimumPayment = input.MinimumPayment;
            double inflationRate = growthRates.inflation;
            double cashAsset = input.CashAsset;
            double propertyAsset = input.PropertyAsset;
            double investmentAsset = input.InvestmentsAsset;
            double mortgageLiability = input.MortgageLiability;
            double otherLoansLiability = input.OtherLoansLiability;
            double debtsLiability = input.OtherDebtsLiability;
            double interestRate = input.InterestRate * 0.01D; // Convert from percentage to decimal
            double monthlyInterestRate = interestRate / 12;
            double investmentGrowthRate = input.InvestmentGrowthRate * 0.01D; // Convert from percentage to decimal
            double monthlyInvestmentGrowthRate = investmentGrowthRate / 12;

            // Calculations
            double monthlyLoanPayment = Math.Round(calc.GetMonthlyLoanPayment(loanAmount, monthlyInterestRate, termInMonths, minimumPayment), 2, MidpointRounding.AwayFromZero);
            double monthlyInvestmentPayment = Math.Round(calc.GetMonthlyInvestmentPayment(monthlyLoanPayment, discretionaryIncome), 2);
            double loanInterest = Math.Round(calc.GetLoanInterest(loanAmount, termInMonths, monthlyLoanPayment), 2);
            double projectedInvestmentTotal = Math.Round(calc.GetProjectedInvestment(monthlyInvestmentPayment, monthlyInvestmentGrowthRate, termInMonths), 2);
            double returnOnInvestment = Math.Round(calc.GetReturnOnInvestment(monthlyInvestmentPayment, termInMonths, projectedInvestmentTotal), 2);
            double suggestedInvestmentAmount = Math.Round(calc.GetSuggestedInvestment(loanInterest, monthlyInvestmentGrowthRate, termInMonths), 2);
            List<double> remainingLoanBalances = calc.GetRemainingLoanBalances(loanAmount, monthlyInterestRate, monthlyLoanPayment, termInMonths);
            List<double> remainingLoanBalancesYearly = new List<double>(); // Change remainingLoanBalances to yearly instead of monthly
            remainingLoanBalancesYearly = remainingLoanBalances.Where((b, i) => i == 0 || (i + 1) % 12 == 0).ToList();
            List<double> netWorth = calc.GetProjectedNetWorth(cashAsset, propertyAsset, investmentAsset, mortgageLiability, otherLoansLiability, debtsLiability, termInYears, monthlyInvestmentPayment, monthlyLoanPayment, loanAmount, interestRate, inflationRate, investmentGrowthRate);

            // Bind output
            outputModel.MonthlyPaymentToLoan = monthlyLoanPayment;
            outputModel.MonthlyPaymentToInvest = monthlyInvestmentPayment;
            outputModel.InterestPaid = loanInterest;
            outputModel.InvestmentValue = projectedInvestmentTotal;
            outputModel.ReturnOnInvestment = returnOnInvestment;
            outputModel.SuggestedInvestmentAmount = suggestedInvestmentAmount;
            outputModel.RemainingLoanBalances = remainingLoanBalancesYearly;
            outputModel.NetWorth = netWorth;

            return outputModel;
        }
    }
}
