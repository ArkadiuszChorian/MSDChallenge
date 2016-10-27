using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataTransform.Models
{
    class Song
    {
        [BsonId]
        public string Id { get; set; }
        public string ArtistId { get; set; }
        public string Name { get; set; }
    }
}
