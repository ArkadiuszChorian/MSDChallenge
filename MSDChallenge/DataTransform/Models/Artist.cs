using MongoDB.Bson.Serialization.Attributes;

namespace DataTransform.Models
{
    public class Artist
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
