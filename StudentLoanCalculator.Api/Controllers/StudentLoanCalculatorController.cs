using Microsoft.AspNetCore.Mvc;
using StudentLoanCalculator.Domain;

namespace StudentLoanCalculator___Team_1.Controllers
{
    [Route("[controller]")]
    public class StudentLoanCalculatorController : Controller
    {
        private ILoanCalculator calc;

        public StudentLoanCalculatorController(ILoanCalculator calc)
        {
            this.calc = calc;
        }

        public IActionResult Index()
        {
            return Json("From StudentLoanCalculatorController");
        }

        [HttpGet("GetInterest")]
        public double GetInterest(int years, double interestRate)
        {
            return calc.GetInterest(years, interestRate);
        }

        [HttpGet("GetMonthlyPayment")]
        public double GetMonthlyPayment(double loanCost, int yearsRemaining, double interestRate)
        {
            decimal monthlyPayment = (decimal) calc.GetMonthlyPayment(loanCost, yearsRemaining, interestRate);
            monthlyPayment = decimal.Round(monthlyPayment, 2);
            return (double) monthlyPayment;
        }
    }
}
