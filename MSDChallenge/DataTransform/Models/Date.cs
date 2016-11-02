using MongoDB.Bson.Serialization.Attributes;

namespace DataTransform.Models
{
    public class Date
    {
        [BsonId]
        public string Id { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
