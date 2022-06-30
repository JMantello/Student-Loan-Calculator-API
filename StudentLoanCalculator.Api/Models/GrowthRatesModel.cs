using MongoDB.Bson.Serialization.Attributes;

namespace StudentLoanCalculator.Api.Models
{
    public class GrowthRatesModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string id { get; set; }
        public double inflation { get; set; }
        public double conservative { get; set; }
        public double moderate { get; set; }
        public double aggressive { get; set; }

    }
}
