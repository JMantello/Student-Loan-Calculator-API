using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using StudentLoanCalculator.Api.Data;
using StudentLoanCalculator.Api.Models;
using StudentLoanCalculator.Domain;

namespace StudentLoanCalculator_Team_1.Controllers
{
    [Route("[controller]")]
    public class StudentLoanCalculatorController : Controller
    {
        private readonly ILoanCalculator loanCalculator;
        private readonly MongoCRUD context;

        private GrowthRatesModel? growthRates { get; set; }

        public StudentLoanCalculatorController(ILoanCalculator loanCalculator, MongoCRUD context)
        {
            this.loanCalculator = loanCalculator;
            this.context = context;
            growthRates = null;
        }

        public IActionResult Index()
        {
            return Json("Student Loan Calculator - Team 1");
        }

        [HttpGet("Calculate")]
        public OutputModel Calculate(InputModel input)
        {
            // Extract this business logic to Calculator
            OutputModel outputModel = new OutputModel();

            // Validate input
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid");
                return outputModel; // Validation errors return a 400 error code
            }

            if (input.TermInYears <= 0)
            {
                Console.WriteLine("Loan period must be greater than 0");
                return outputModel;
            }

            // Destructure input
            double discretionaryIncome = input.MonthlyDiscretionaryIncome;
            double loanAmount = input.LoanAmount;
            int termInYears = input.TermInYears;
            int termInMonths = termInYears * 12;
            double interestRate = input.InterestRate * 0.01D; // Convert from percentage to decimal
            double monthlyInterestRate = interestRate / 12;
            InvestmentRiskKind investmentRisk = input.InvestmentRisk;

            // Get growth rates
            if (context == null)
            {
                growthRates = GrowthRatesModel.DefaultGrowthRates[investmentRisk];
            }
            else
            {
                // Get growth rates from Db
                growthRates = context.LoadGrowthRates<GrowthRatesModel>(investmentRisk);
            } 

            double investmentGrowthRate = growthRates.average;
            double monthlyInvestmentGrowthRate = investmentGrowthRate / 12;

            // Calculations
            double monthlyPaymentToLoan = MonthlyLoanPayment(loanAmount, monthlyInterestRate, termInMonths);
            double monthlyPaymentToInvest = MonthlyInvestmentPayment(monthlyPaymentToLoan, discretionaryIncome);
            double interestPaid = LoanInterest(loanAmount, termInMonths, monthlyPaymentToLoan);
            double projectedInvestment = ProjectedInvestment(monthlyPaymentToInvest, monthlyInvestmentGrowthRate, termInMonths);
            double returnOnInvestment = ReturnOnInvestment(monthlyPaymentToInvest, termInMonths, projectedInvestment);
            double suggestedInvestmentAmount = SuggestedInvestment(interestPaid, monthlyInvestmentGrowthRate, termInMonths);
            List<double> monthlyRemainingLoanBalances = MonthlyRemainingLoanBalances(loanAmount, monthlyInterestRate, monthlyPaymentToLoan, termInMonths);
            List<double> yearlyRemainingLoanBalances = YearlyRemainingLoanBalances(loanAmount, monthlyInterestRate, monthlyPaymentToLoan, termInMonths);
            List<double> monthlyInvestmentGrowth = MonthlyInvestmentGrowth(monthlyPaymentToInvest, monthlyInvestmentGrowthRate, termInMonths);
            List<double> yearlyInvestmentGrowth = YearlyInvestmentGrowth(monthlyPaymentToInvest, monthlyInvestmentGrowthRate, termInMonths);
            List<double> monthlyNetWorthImpact = MonthlyNetWorthImpact(monthlyRemainingLoanBalances.ToArray(), monthlyInvestmentGrowth.ToArray()); // Unused
            List<double> yearlyNetWorthImpact = YearlyNetWorthImpact(monthlyRemainingLoanBalances.ToArray(), monthlyInvestmentGrowth.ToArray());

            // Bind output
            outputModel.MonthlyPaymentToLoan = monthlyPaymentToLoan;
            outputModel.MonthlyPaymentToInvest = monthlyPaymentToInvest;
            outputModel.InterestPaid = interestPaid;
            outputModel.TotalPaidToLoan = loanAmount + interestPaid;
            outputModel.ProjectedInvestment = projectedInvestment;
            outputModel.ReturnOnInvestment = returnOnInvestment;
            outputModel.SuggestedInvestmentAmount = suggestedInvestmentAmount;
            outputModel.YearlyRemainingLoanBalances = yearlyRemainingLoanBalances;
            outputModel.YearlyInvestmentGrowth = yearlyInvestmentGrowth;
            outputModel.YearlyNetWorthImpact = yearlyNetWorthImpact;
            outputModel._RiskPercentageHigh = growthRates.high;
            outputModel._RiskPercentageLow = growthRates.low;

            return outputModel;
        }

        [HttpGet("MonthlyLoanPayment")]
        public double MonthlyLoanPayment(double loanAmount, double monthlyInterestRate, int termInMonths)
        {
            double monthlyLoanPayment = Math.Round(loanCalculator.MonthlyLoanPayment(loanAmount, monthlyInterestRate, termInMonths), 2, MidpointRounding.AwayFromZero);
            return monthlyLoanPayment;
        }

        [HttpGet("MonthlyInvestmentPayment")]
        public double MonthlyInvestmentPayment(double monthlyLoanPayment, double discretionaryIncome)
        {
            double monthlyInvestmentPayment = Math.Round(loanCalculator.MonthlyInvestmentPayment(monthlyLoanPayment, discretionaryIncome), 2);
            return monthlyInvestmentPayment;
        }

        [HttpGet("LoanInterest")]
        public double LoanInterest(double loanAmount, int termInMonths, double monthlyLoanPayment)
        {
            double loanInterest = Math.Round(loanCalculator.LoanInterest(loanAmount, termInMonths, monthlyLoanPayment), 2);
            return loanInterest;
        }

        [HttpGet("ProjectedInvestment")]
        public double ProjectedInvestment(double monthlyInvestmentPayment, double monthlyInvestmentGrowthRate, int termInMonths)
        {
            double projectedInvestmentTotal = Math.Round(loanCalculator.ProjectedInvestment(monthlyInvestmentPayment, monthlyInvestmentGrowthRate, termInMonths), 2);
            return projectedInvestmentTotal;
        }

        [HttpGet("ReturnOnInvestment")]
        public double ReturnOnInvestment(double monthlyInvestmentPayment, int termInMonths, double projectedInvestmentTotal)
        {
            double returnOnInvestment = Math.Round(loanCalculator.ReturnOnInvestment(monthlyInvestmentPayment, termInMonths, projectedInvestmentTotal), 2);
            return returnOnInvestment;
        }

        [HttpGet("SuggestedInvestment")]
        public double SuggestedInvestment(double loanInterest, double monthlyInvestmentGrowthRate, double termInMonths)
        {
            // Finds the monthly amount to invest so that at the end of the term, the calculated growth will be equal to the interest cost from the loan.
            double suggestedInvestmentAmount = Math.Round(loanCalculator.SuggestedInvestment(loanInterest, monthlyInvestmentGrowthRate, termInMonths), 2);
            return suggestedInvestmentAmount;
        }

        [HttpGet("MonthlyRemainingLoanBalances")]
        public List<double> MonthlyRemainingLoanBalances(double loanAmount, double monthlyInterestRate, double monthlyLoanPayment, int termInMonths) // InputModel.LoanInfoQuery query
        {
            List<double> remainingLoanBalances = loanCalculator.RemainingLoanBalances(loanAmount, monthlyInterestRate, monthlyLoanPayment, termInMonths);

            return remainingLoanBalances;
        }

        [HttpGet("YearlyRemainingLoanBalances")]
        public List<double> YearlyRemainingLoanBalances(double loanAmount, double monthlyInterestRate, double monthlyLoanPayment, int termInMonths)
        {
            return ConvertToYearly(MonthlyRemainingLoanBalances(loanAmount, monthlyInterestRate, monthlyLoanPayment, termInMonths));
        }

        [HttpGet("MonthlyInvestmentGrowth")]
        public List<double> MonthlyInvestmentGrowth(double monthlyInvestment, double monthlyInvestmentGrowthRate, int months)
        {
            List<double> monthlyInvestmentGrowth = loanCalculator.MonthlyInvestmentGrowth(monthlyInvestment, monthlyInvestmentGrowthRate, months);
            return monthlyInvestmentGrowth;
        }

        [HttpGet("YearlyInvestmentGrowth")]
        public List<double> YearlyInvestmentGrowth(double monthlyInvestment, double monthlyInvestmentGrowthRate, int months)
        {
            return ConvertToYearly(MonthlyInvestmentGrowth(monthlyInvestment, monthlyInvestmentGrowthRate, months));
        }

        [HttpGet("MonthlyNetWorthImpact")]
        public List<double> MonthlyNetWorthImpact(double[] liabilityRemaining, double[] assets)
        {
            List<double> monthlyNetWorthImpact = loanCalculator.MonthlyNetWorthImpact(liabilityRemaining, assets);
            return monthlyNetWorthImpact;
        }

        [HttpGet("YearlyNetWorthImpact")]
        public List<double> YearlyNetWorthImpact(double[] liabilityRemaining, double[] assets)
        {
            return ConvertToYearly(MonthlyNetWorthImpact(liabilityRemaining, assets));
        }

        private List<double> ConvertToYearly(List<double> monthlyList)
        {
            List<double> yearly = new List<double>();
            yearly = monthlyList.Where((b, i) => i == 0 || i % 12 == 0).ToList();

            return yearly;
        }
    }


}
