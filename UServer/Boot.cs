using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UNetwork;
using UNetwork.Network.Utility;
using UNetwork.Utility;
using UServer.Database;
using UServer.Sessions;

namespace UServer
{
    public class Boot
    {
        static private NetworkServer server;

        static void Main(string[] args)
        {
            Debug.Log("Initializing the database adapter.");
            //This is a vital thing in order to proceed the queries
            Adapter.Initialize("localhost", 3306, "root", "", "newgame");
            
            Debug.Log("Starting our realtime server.");
            server = new NetworkServer("127.0.0.1", 3000);
            server.RegisterHandler(MsgType.Connect, OnPlayerConnection);
            server.RegisterHandler(MsgType.Disconnect, OnPlayerDisconnection);
            server.RegisterHandler(NetworkOpcodes.LOGIN_REQUEST, OnPlayerAuthRequest);
            server.RegisterHandler(NetworkOpcodes.PLAYER_CARDS_REQUEST, OnPlayerCardsRequest);
            server.Start();

        }

        private static void OnPlayerCardsRequest(NetworkMessage netMsg)
        {
            Debug.Log("Player has asked for cards");
            
        }

        //@TODO: READ THIS
        //THIS LOADS PROFILE DATA ONLY BUT LETS MAKE COUPLE REQUESTS IN CASE
        //ITS A SUCCESS AND ASK FOR THE PLAYER CARDS, PLAYER ACHIVE ETC
        private static void OnPlayerAuthRequest(NetworkMessage netMsg)
        {
            Debug.Log("A player has requested to login");
            var authdata = netMsg.ReadMessage<AuthData>();
            Debug.Log("Password received: " + authdata.password);

            //Process now the request
            var profileData = Users.GetProfileWithAuth(authdata.username, authdata.password);
            if (profileData != null)
            {
                if (SessionManager.Exists(profileData.id))
                {
                    SessionManager.Remove(profileData.id);
                    Debug.Warning("Removed a session active with the account asking to Auth");
                }
                SessionManager.New(netMsg.Sender, profileData.id);
                server.SendToClient(netMsg.Sender, NetworkOpcodes.LOGIN_SUCCESS, profileData);
            }
            else
            {
                server.SendToClient(netMsg.Sender, NetworkOpcodes.LOGIN_FAIL, new ActionError { code = 100, message = "User does not exist or password is wrong" });
            }
        }

        private static void OnPlayerConnection(NetworkMessage netMsg)
        {
            Debug.Log($"A player with id {netMsg.Sender.connectionId} has joined");
            //playerSessions.Add(new PlayerSession(netMsg.Sender)); doesn't make sense
            //because he just gets a valid session after a login
        }

        private static void OnPlayerDisconnection(NetworkMessage netMsg)
        {
            Debug.Log($"A player with id {netMsg.Sender.connectionId} has disconnected from the server");
            SessionManager.Remove(netMsg.Sender.connectionId);
        }
    }
}
