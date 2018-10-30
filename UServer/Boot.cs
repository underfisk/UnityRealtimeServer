using Networking.Core;
using Networking.Utility;
using UnityServer.Database;
using UnityServer.EventServer.GameServer;
using UnityServer.EventServer.LoginServer;
using UnityServer.EventServer.StoreServer;

namespace UnityServer
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
            LoginCoordinator loginServer = new LoginCoordinator(server);
            StoreCoordinator storeServer = new StoreCoordinator(server);
            GameCoordinator gameServer = new GameCoordinator(server);

            //Server must start after all sub-servers being initialized
            server.Start();

        }


    }
}
