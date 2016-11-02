using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using DataTransform.Comparers;
using DataTransform.Models;
using MongoDB.Driver;

namespace DataTransform
{
    class Program
    {
        public static Stopwatch Stoper { get; set; } = new Stopwatch();
        public static Dictionary<string, Artist> ArtistsSet = new Dictionary<string, Artist>(1000000);
        public static Dictionary<string, Song> SongsSet = new Dictionary<string, Song>(1000000);
        public static HashSet<Listen> ListensSet = new HashSet<Listen>();       
        public static HashSet<User> UsersSet = new HashSet<User>(new UserEqualityComparer());
        public static HashSet<FullListen> FullListensSet = new HashSet<FullListen>();
        private const string UniqueTracksFileName = "unique_tracks.txt";
        private const string ListensFileName = "triplets_sample_20p.txt";

        public static IMongoClient Client;
        public static IMongoDatabase Database;
        public static IMongoCollection<Artist> Artists;
        public static IMongoCollection<Song> Songs;
        public static IMongoCollection<User> Users;
        public static IMongoCollection<Listen> Listens;
        public static IMongoCollection<FullListen> FullListens;
        public static IMongoCollection<Statistics> Statistics;
        public static Statistics Stats = new Statistics();

        static void Main(string[] args)
        {
            Client = new MongoClient();
            Database = Client.GetDatabase("MSDChallenge");
            Artists = Database.GetCollection<Artist>("Artists");
            Songs = Database.GetCollection<Song>("Songs");
            Users = Database.GetCollection<User>("Users");
            Listens = Database.GetCollection<Listen>("Listens");
            FullListens = Database.GetCollection<FullListen>("FullListens");
            Statistics = Database.GetCollection<Statistics>("Statistics");

            TransformData();

            Statistics.InsertOne(Stats);
                  
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static void TransformData()
        {
            Stoper.Reset();
            Stoper.Start();
            LoadUniqueTracksFromFile();
            Stoper.Stop();
            Stats.SeparatedSchemaUniqueTracksInMemoryElapsedSeconds += Stoper.Elapsed.Seconds;
            Stats.SeparatedSchemaTotalElapsedSeconds += Stoper.Elapsed.Seconds;

            Stoper.Reset();
            Stoper.Start();
            //InsertUniqueTracksToDb();
            Stoper.Stop();
            Stats.SeparatedSchemaUniqueTracksToDbElapsedSeconds += Stoper.Elapsed.Seconds;
            Stats.SeparatedSchemaTotalElapsedSeconds += Stoper.Elapsed.Seconds;

            Stoper.Reset();
            Stoper.Start();
            LoadListensFromFile();
            Stoper.Stop();
            Stats.SeparatedSchemaListensInMemoryElapsedSeconds += Stoper.Elapsed.Seconds;
            Stats.SeparatedSchemaTotalElapsedSeconds += Stoper.Elapsed.Seconds;

            Stoper.Reset();
            Stoper.Start();
            //InsertListensAndUsersToDb();
            Stoper.Stop();
            Stats.SeparatedSchemaListensToDbElapsedSeconds += Stoper.Elapsed.Seconds;
            Stats.SeparatedSchemaTotalElapsedSeconds += Stoper.Elapsed.Seconds;

            Stoper.Reset();
            Stoper.Start();
            LoadFullListens();
            Stoper.Stop();
            Stats.PlainSchemaInMemoryElapsedSeconds += Stoper.Elapsed.Seconds;
            Stats.PlainSchemaTotalElapsedSeconds += Stoper.Elapsed.Seconds;

            Stoper.Reset();
            Stoper.Start();
            //InsertFullListensToDb();
            Stoper.Stop();
            Stats.PlainSchemaToDbElapsedSeconds += Stoper.Elapsed.Seconds;
            Stats.PlainSchemaTotalElapsedSeconds += Stoper.Elapsed.Seconds;
        }

        private static void LoadUniqueTracksFromFile()
        {          
            try
            {
                using (var tracksStreamReader = new StreamReader(UniqueTracksFileName))
                {
                    string line;

                    while ((line = tracksStreamReader.ReadLine()) != null)
                    {
                        //Index:        0,          1,          2,              3
                        //Track info:   Artist ID,  Song ID,    Artist Name,    Song Name

                        var trackInfo = line.Split(new[] { "<SEP>" }, StringSplitOptions.None);
                       
                        if (!SongsSet.ContainsKey(trackInfo[1]))
                            SongsSet.Add(trackInfo[1], new Song { Id = trackInfo[1], ArtistId = trackInfo[0], Name = trackInfo[3] });
                        if (!ArtistsSet.ContainsKey(trackInfo[0]))
                            ArtistsSet.Add(trackInfo[0], new Artist { Id = trackInfo[0], Name = trackInfo[2] });  
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("SUCCES: Unique tracks loaded!");
        }

        private static void InsertUniqueTracksToDb()
        {
            Songs.InsertMany(SongsSet.Values, new InsertManyOptions {IsOrdered = false});
            Console.WriteLine("SUCCES: Songs inserted!");
            Artists.InsertMany(ArtistsSet.Values);
            Console.WriteLine("SUCCES: Artists inserted!");
        }

        private static void LoadListensFromFile()
        {
            try
            {
                using (var listensStreamReader = new StreamReader(ListensFileName))
                {
                    string line;

                    while ((line = listensStreamReader.ReadLine()) != null)
                    {
                        //Index:        0,          1,          2,       
                        //Listen info:  User ID,    Song ID,    Listen Date  
                        var listenInfo = line.Split(new[] { "<SEP>" }, StringSplitOptions.None);
                        var date = UnixTimeStampToDateTime(long.Parse(listenInfo[2]));
                        var artistId = SongsSet[listenInfo[1]].ArtistId;

                        ListensSet.Add(new Listen {ArtistId = artistId, UserId = listenInfo[0], SongId = listenInfo[1], Day = date.Day, Month = date.Month, Year = date.Year});

                        if (UsersSet.Contains(new User {Id = listenInfo[0]})) continue;
                        UsersSet.Add(new User { Id = listenInfo[0] });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("SUCCES: Listens loaded!");
        }

        private static void InsertListensAndUsersToDb()
        {
            Listens.InsertMany(ListensSet);
            Console.WriteLine("SUCCES: Listens inserted!");
            Users.InsertMany(UsersSet);
            Console.WriteLine("SUCCES: Users inserted!");
        }

        private static void LoadFullListens()
        {
            foreach (var listen in ListensSet)
            {
                //var fullListen = new FullListen {ArtistId =  listen.ArtistId, ArtistName = ArtistsSet[listen.ArtistId].Name, Day = listen.Day, Month = listen.Month, Year = listen.Year, UserId = listen.UserId, SongId = listen.SongId, SongName = SongsSet[listen.SongId].Name};
                FullListensSet.Add(new FullListen
                {
                    ArtistId = listen.ArtistId,
                    ArtistName = ArtistsSet[listen.ArtistId].Name,
                    Day = listen.Day,
                    Month = listen.Month,
                    Year = listen.Year,
                    UserId = listen.UserId,
                    SongId = listen.SongId,
                    SongName = SongsSet[listen.SongId].Name
                });
            }

            Console.WriteLine("SUCCES: Full listens loaded!");
        }

        private static void InsertFullListensToDb()
        {
            FullListens.InsertMany(FullListensSet);
            Console.WriteLine("SUCCES: Full listens inserted!");
        }

        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}

//var query = (from song in songs.AsQueryable()
//             join listen in listens.AsQueryable() on song.Id equals listen.SongId into joined
//             group joined by song.Name into grouped
//             select new { Name = grouped.Key, Count = grouped.Count() }).OrderByDescending(q => q.Count);                   

//var query = from song in songs.AsQueryable()
//    select new {N = song.Name};

//var results = query.Take(5).ToList();

//foreach (var result in results)
//{
//    Console.WriteLine(result.N);
//}

//var results = query.Take(5).ToList();

//foreach (var result in results)
//{
//    Console.WriteLine(result.Name + " " + result.Count);
//}