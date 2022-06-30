using StudentLoanCalculator.Domain;
using StudentLoanCalculator.Api.Identity;
using StudentLoanCalculator.Api.Models;
using StudentLoanCalculator.Api.Data;

namespace StudentLoanCalculator.Api
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<ILoanCalculator, LoanCalculator>();

            // Configure database
            //MongoCRUD db = new MongoCRUD("StudentLoanCalculatorDb");
            //builder.Services.AddSingleton(db);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }

        private void InsertDefaultUser()
        {
            MongoCRUD db = new MongoCRUD("StudentLoanCalculatorDb");

            UserModel defaultUser = new UserModel();
            defaultUser.FirstName = "Jonathan";
            defaultUser.LastName = "Mantello";
            defaultUser.EmailAddress = "jmantello@emailadress.com";
            defaultUser.SavedCalculations = new List<SavedCalculationModel>();

            InputModel inputModel = new InputModel();
            inputModel.DiscretionaryIncome = 1500;
            inputModel.LoanAmount = 40000;
            inputModel.InterestRate = 5.8;
            inputModel.TermInYears = 20;
            inputModel.MinimumPayment = 200;
            inputModel.InvestmentGrowthRate = 10.5;
            inputModel.CashAsset = 20000;
            inputModel.PropertyAsset = 400000;
            inputModel.InvestmentsAsset = 0;
            inputModel.MortgageLiability = 0;
            inputModel.OtherLoansLiability = 100000;
            inputModel.OtherDebtsLiability = 0;

            SavedCalculationModel savedCalculation = new SavedCalculationModel();
            savedCalculation.Name = "My saved calculation";
            savedCalculation.InputModel = inputModel;

            defaultUser.SavedCalculations.Add(savedCalculation);

            db.InsertRecord("Users", defaultUser);
        }
    }
}