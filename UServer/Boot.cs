using System.Threading;
using UNetwork;
using UServer.Coordinators;
using UServer.Database;

namespace UServer
{
    public class Boot
    {
        static void Main(string[] args)
        {
            Debug.Log("Initializing the database adapter.");
            //This is a vital thing in order to proceed the queries
            Adapter.Initialize("localhost", 3306, "root", "", "newgame");
            
            Debug.Log("Starting our realtime server.");
            var server = new NetworkServer("127.0.0.1", 3000);

            LoginServer loginServer = new LoginServer(server);
            StoreServer storeServer = new StoreServer(server);
            GameServer gameServer = new GameServer(server);

            //Server must start after all sub-servers being initialized
            server.Start();


        }


    }
}
