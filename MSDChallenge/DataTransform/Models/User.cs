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
    public class User : IEntity<string>
    {
        [BsonId]
        public string Id { get; set; }
    }
}
