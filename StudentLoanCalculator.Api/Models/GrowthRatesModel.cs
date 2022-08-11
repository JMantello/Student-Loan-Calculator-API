using MongoDB.Bson.Serialization.Attributes;

namespace StudentLoanCalculator.Api.Models
{
    public class GrowthRatesModel
    {
        private static readonly GrowthRatesModel Conservative = new()
        {
            riskKind = InvestmentRiskKind.Conservative,
            average = 0.149,
            low = -0.341,
            high = 0.343
        };

        private static readonly GrowthRatesModel Moderate = new GrowthRatesModel()
        {
            riskKind = InvestmentRiskKind.Moderate,
            average = 0.173,
            low = -0.438,
            high = 0.526
        };

        private static readonly GrowthRatesModel Aggressive = new GrowthRatesModel()
        {
            riskKind = InvestmentRiskKind.Aggressive,
            average = 0.238,
            low = -0.417,
            high = 1.017
        };

        public static readonly Dictionary<InvestmentRiskKind, GrowthRatesModel> DefaultGrowthRates = new Dictionary<InvestmentRiskKind, GrowthRatesModel>()
        {
            { InvestmentRiskKind.Conservative, Conservative } ,
            { InvestmentRiskKind.Moderate, Moderate },
            { InvestmentRiskKind.Aggressive, Aggressive }
        };

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string id { get; set; }

        public InvestmentRiskKind riskKind { get; set; }
        public double average { get; set; }
        public double low { get; set; }
        public double high { get; set; }
    }
}
