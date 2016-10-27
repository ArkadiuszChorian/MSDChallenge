using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;

namespace DataTransform.Models
{
    public class Listen : IEntity<string>
    {
        [BsonId]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ArtistId { get; set; }
        public string SongId { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
