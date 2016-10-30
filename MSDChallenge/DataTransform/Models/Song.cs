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
    public class Song : IEntity<string>
    {
        [BsonId]
        public string Id { get; set; }
        public string ArtistId { get; set; }
        public string Name { get; set; }
        [BsonIgnore]
        public long NumberOfListens { get; set; } = 0;
    }
}
