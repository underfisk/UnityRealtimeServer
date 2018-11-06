using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Networking.Core;
using MasterServer.Database;
using MasterServer.EventServer.GameServer;
using MasterServer.EventServer.LoginServer;
using MasterServer.EventServer.StoreServer;
using Debug = Networking.Utility.Debug;

namespace MasterServer
{
    public class Boot
    {
        static void Main(string[] args)
        {
            Debug.Log("Initializing the database adapter.");
            //This is a vital thing in order to proceed the queries
            Adapter.Initialize("localhost", 3306, "root", "", "newgame");
            
            Debug.Log("Starting our realtime server.");
            NetworkServer server = new NetworkServer("127.0.0.1", 3000);

            //Coordinators initialization
            //@todo Make them different threads not only instances
            LoginCoordinator loginServer = new LoginCoordinator(server);
            StoreCoordinator storeServer = new StoreCoordinator(server);
            GameCoordinator gameServer = new GameCoordinator(server);

            //Show repo information
            DisplayRepoInformation();
            //Display Build Version
            DisplayBuildVersion();
            //Server must start after all sub-servers being initialized
            server.Start();

        }

        /// <summary>
        /// Diplay sthe build version saved at the assembly
        /// </summary>
        static void DisplayBuildVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            Debug.Log($"Build Version: {version} ");
            DisplayBuildDate();
            
        }

        /// <summary>
        /// Displays the build date
        /// </summary>
        static void DisplayBuildDate()
        {
            var date = GetLinkerTime(Assembly.GetExecutingAssembly(), TimeZoneInfo.Local);
            Debug.Log($"Build date: {date}");
        }
        
        /// <summary>
        /// Returns assembly build date to normal date
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        static DateTime GetLinkerTime(Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

        public static void DisplayRepoInformation()
        {
            Debug.Log("Unity server created by Enigma \n Repository: github.com/underfisk/UnityRealtimeServer");
        }
    }
}
