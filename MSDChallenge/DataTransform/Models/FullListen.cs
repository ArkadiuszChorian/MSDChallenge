using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace DataTransform.Models
{
    public class FullListen
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ArtistId { get; set; }
        public string SongId { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string SongName { get; set; }
        public string ArtistName { get; set; }
    }
}
