using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using DataTransform.Comparers;
using DataTransform.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DataTransform
{
    class Program
    {
        public static Stopwatch Stoper { get; set; } = new Stopwatch();
        //public static ArtistEqualityComparer Comparer = new ArtistEqualityComparer();
        public static HashSet<Artist> ArtistsSet = new HashSet<Artist>(new ArtistEqualityComparer());
        public static HashSet<Song> SongsSet = new HashSet<Song>();
        public static HashSet<Listen> ListensSet = new HashSet<Listen>();
        public static HashSet<User> UsersSet = new HashSet<User>(new UserEqualityComparer());
        public static long ProgramElapsedSeconds = 0;
        private const string UniqueTracksFileName = "unique_tracks.txt";
        private const string ListensFileName = "triplets_sample_20p.txt";

        static void Main(string[] args)
        {   
            Stoper.Start();

            LoadUniqueTracksInfoAndInsertInDb();  

            Stoper.Stop();
            ProgramElapsedSeconds += Stoper.Elapsed.Seconds;
            Stoper.Reset();
            Stoper.Start();

            LoadListensFromFileAndInsertInDb();

            Stoper.Stop();
            ProgramElapsedSeconds += Stoper.Elapsed.Seconds;
            Stoper.Reset();

            Console.WriteLine("Done");
            Console.ReadKey();
        }        

        private static void LoadUniqueTracksInfoAndInsertInDb()
        {
            //var comparer = new ArtistEqualityComparer();
            //var artistsSet = new HashSet<Artist>(comparer);
            //var songsSet = new HashSet<Song>();

            //Stoper.Start();

            try
            {
                using (var tracksStreamReader = new StreamReader(UniqueTracksFileName))
                {
                    string line;

                    while ((line = tracksStreamReader.ReadLine()) != null)
                    {
                        //Index:        0,          1,          2,              3
                        //Track info:   Song ID,   Artist ID,  Artist Name,    Song Name
                        var trackInfo = line.Split(new[] { "<SEP>" }, StringSplitOptions.None);

                        SongsSet.Add(new Song { Id = trackInfo[0], ArtistId = trackInfo[1], Name = trackInfo[3] });

                        if (!ArtistsSet.Contains(new Artist { Id = trackInfo[1] }))
                            ArtistsSet.Add(new Artist { Id = trackInfo[1], Name = trackInfo[2] });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            //Console.WriteLine();
            //Console.WriteLine();
            //Stoper.Stop();
            //Console.WriteLine(Stoper.Elapsed.Seconds);
            //Stoper.Reset();
            //Stoper.Start();

            DAL.Instance.Songs.Add(SongsSet);
            DAL.Instance.Artists.Add(ArtistsSet);

            //Stoper.Stop();
            //Console.WriteLine(Stoper.Elapsed.Seconds);
            //Stoper.Reset();
        }

        private static void LoadListensFromFileAndInsertInDb()
        {
            //var listensSet = new HashSet<Listen>();
            //var usersSet = new HashSet<User>();

            //Stoper.Start();

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

                        //var song = SongsSet.FirstOrDefault(s => s.Id == listenInfo[1]);
                        var song = DAL.Instance.Songs.GetById(listenInfo[1]); //7647 addings per sec
                        var artistId = string.Empty;
                        if (song != null)
                            artistId = song.ArtistId;

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
            //Console.WriteLine();
            //Console.WriteLine();
            //Stoper.Stop();
            //Console.WriteLine(Stoper.Elapsed.Seconds);
            //Stoper.Reset();
            //Stoper.Start();
            
            DAL.Instance.Users.Add(UsersSet);
            DAL.Instance.Listens.Add(ListensSet);

            //Stoper.Stop();
            //Console.WriteLine(Stoper.Elapsed.Seconds);
            //Stoper.Reset();
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
