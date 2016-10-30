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
        public static Dictionary<string, Artist> ArtistsSet = new Dictionary<string, Artist>(1000000);
        public static Dictionary<string, Song> SongsSet = new Dictionary<string, Song>(1000000);
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
            
            DAL.Instance.Songs.Add(SongsSet.Values);
            DAL.Instance.Artists.Add(ArtistsSet.Values);

            Console.WriteLine("First part done!");
        }

        private static void LoadListensFromFileAndInsertInDb()
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

            DAL.Instance.Listens.Add(ListensSet);
            DAL.Instance.Users.Add(UsersSet);      
               
        }

        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}