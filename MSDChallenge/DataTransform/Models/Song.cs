using MongoDB.Bson.Serialization.Attributes;

namespace DataTransform.Models
{
    public class Song
    {
        [BsonId]
        public string Id { get; set; }
        public string ArtistId { get; set; }
        public string Name { get; set; }
        [BsonIgnore]
        public long NumberOfListens { get; set; } = 0;
    }
}
