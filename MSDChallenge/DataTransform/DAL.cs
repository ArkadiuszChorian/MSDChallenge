using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransform.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoRepository;

namespace DataTransform
{
    public sealed class DAL
    {
        private readonly MongoDatabase _db;

        private static readonly Lazy<DAL> Lazy =
        new Lazy<DAL>(() => new DAL());
        public static DAL Instance => Lazy.Value;

        private DAL()
        {
            _db = Artists.Collection.Database;
        }

        public T Fetch<T>(MongoDBRef reference)
        {
            return _db.FetchDBRefAs<T>(reference);
        }

        public MongoRepository<Artist> Artists { get; set; } = new MongoRepository<Artist>();
        public MongoRepository<Date> Dates { get; set; } = new MongoRepository<Date>();
        public MongoRepository<Listen> Listens { get; set; } = new MongoRepository<Listen>();
        public MongoRepository<Song> Songs { get; set; } = new MongoRepository<Song>();
        public MongoRepository<User> Users { get; set; } = new MongoRepository<User>();
    }
}
