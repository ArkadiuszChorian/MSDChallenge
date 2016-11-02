using MongoDB.Bson.Serialization.Attributes;

namespace DataTransform.Models
{
    public class User
    {
        [BsonId]
        public string Id { get; set; }
    }
}
