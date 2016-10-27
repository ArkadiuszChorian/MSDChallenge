using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransform.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DataTransform
{
    class Program
    {
        private static IMongoClient _client;
        private static IMongoDatabase _database;
        private static char[] _separator = {'<', 'S', 'E', 'P', '>'};

        static void Main(string[] args)
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("MSDChallenge");
            Console.WriteLine(_database.DatabaseNamespace);
            //BsonClassMap.RegisterClassMap<Artist>();
            //BsonClassMap.RegisterClassMap<Date>();
            //BsonClassMap.RegisterClassMap<Listen>();
            //BsonClassMap.RegisterClassMap<Song>();
            //BsonClassMap.RegisterClassMap<User>();

            try
            {
                using (var tracksStreamReader = new StreamReader("unique_tracks.txt"))
                {
                    string line;
                    while ((line = tracksStreamReader.ReadLine()) != null)
                    {
                        //Index:        0,          1,          2,              3
                        //Track info:   Song ID,   Artist ID,  Artist Name,    Song Name
                        var trackInfo = line.Split(new[] { "<SEP>" }, StringSplitOptions.None);

                        //Console.WriteLine(trackInfo[0]);
                        //Console.WriteLine(trackInfo[1]);
                        //Console.WriteLine(trackInfo[2]);
                        //Console.WriteLine(trackInfo[3]);

                        var artistCollection = _database.GetCollection<Artist>("artists");
                        var songCollection = _database.GetCollection<Song>("songs");

                        var artist = new Artist { Id = trackInfo[1], Name = trackInfo[2] };
                        var song = new Song { Id = trackInfo[0], ArtistId = trackInfo[1], Name = trackInfo[3] };
                        songCollection.InsertOne(song);
                        artistCollection.InsertOne(artist);

                        //artistCollection.Find(artist => artist.Id == trackInfo[1]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            //try
            //{                
            //    using (var listensStreamReader = new StreamReader("triplets_sample_20p.txt"))
            //    {
            //        //Index:        0,          1,          2,       
            //        //Listen info:  User ID,    Song ID,    Listen Date
            //        var trackInfo = tracksStreamReader.ReadToEnd().Split(Convert.ToChar("<SEP>"));
            //        var listenInfo = listensStreamReader.ReadToEnd().Split(Convert.ToChar("<SEP>"));

            //        var artistCollection = _database.GetCollection<Artist>("artists");
            //        var dateCollection = _database.GetCollection<Date>("dates");
            //        var listenCollection = _database.GetCollection<Listen>("listens");
            //        var songCollection = _database.GetCollection<Song>("songs");
            //        var userCollection = _database.GetCollection<User>("users");

            //        var listenDate = UnixTimeStampToDateTime(long.Parse(listenInfo[2]));

            //        var artist = new Artist { Id = new ObjectId(trackInfo[1]), Name = trackInfo[2] };
            //        var date = new Date { Day = listenDate.Day, Month = listenDate.Month, Year = listenDate.Year };
            //        var user = new User { Id = new ObjectId(listenInfo[0]) };

            //        userCollection.InsertOne(user);
            //        artistCollection.InsertOne(artist);
            //        dateCollection.InsertOne(date);

            //        var song = new Song { Id = new ObjectId(trackInfo[0]), ArtistId = artist.Id, Name = trackInfo[3] };
            //        var listen = new Listen { ArtistId = artist.Id, DateId = date.Id, SongId = new ObjectId(listenInfo[1]), UserId = user.Id };

            //        songCollection.InsertOne(song);
            //        listenCollection.InsertOne(listen);

            //        //Console.WriteLine(line);
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("The file could not be read:");
            //    Console.WriteLine(e.Message);
            //}

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}

//try
//{                
//    using (var listensStreamReader = new StreamReader("triplets_sample_20p.txt"))
//    {
//        //Index:        0,          1,          2,       
//        //Listen info:  User ID,    Song ID,    Listen Date
//        var trackInfo = tracksStreamReader.ReadToEnd().Split(Convert.ToChar("<SEP>"));
//        var listenInfo = listensStreamReader.ReadToEnd().Split(Convert.ToChar("<SEP>"));

//        var artistCollection = _database.GetCollection<Artist>("artists");
//        var dateCollection = _database.GetCollection<Date>("dates");
//        var listenCollection = _database.GetCollection<Listen>("listens");
//        var songCollection = _database.GetCollection<Song>("songs");
//        var userCollection = _database.GetCollection<User>("users");

//        var listenDate = UnixTimeStampToDateTime(long.Parse(listenInfo[2]));

//        var artist = new Artist { Id = new ObjectId(trackInfo[1]), Name = trackInfo[2] };
//        var date = new Date { Day = listenDate.Day, Month = listenDate.Month, Year = listenDate.Year };
//        var user = new User { Id = new ObjectId(listenInfo[0]) };

//        userCollection.InsertOne(user);
//        artistCollection.InsertOne(artist);
//        dateCollection.InsertOne(date);

//        var song = new Song { Id = new ObjectId(trackInfo[0]), ArtistId = artist.Id, Name = trackInfo[3] };
//        var listen = new Listen { ArtistId = artist.Id, DateId = date.Id, SongId = new ObjectId(listenInfo[1]), UserId = user.Id };

//        songCollection.InsertOne(song);
//        listenCollection.InsertOne(listen);

//        //Console.WriteLine(line);
//    }
//}
//catch (Exception e)
//{
//    Console.WriteLine("The file could not be read:");
//    Console.WriteLine(e.Message);
//}
