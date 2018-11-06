using Networking;
using Networking.Core;
using Networking.Sessions;
using Networking.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using MasterServer;
using MasterServer.Database;
using MasterServer.Sessions;

namespace MasterServer.EventServer.LoginServer
{
    /// <summary>
    /// This threaded Server handles all the login/session events
    /// </summary>
    public sealed class LoginCoordinator
    {
        /// <summary>
        /// UNetwork Server Instance booted already in our main function
        /// </summary>
        private NetworkServer Server;

        public LoginCoordinator(NetworkServer runningServer)
        {
            this.Server = runningServer;
            Initialize();
        }

        /// <summary>
        /// Initializes the Server callsback
        /// </summary>
        private void Initialize()
        {
            Server.RegisterHandler(MsgType.Connect, OnPlayerConnection);
            Server.RegisterHandler(MsgType.Disconnect, OnPlayerDisconnection);
            Server.RegisterHandler(LoginEvent.LOGIN_REQUEST, OnPlayerAuthRequest);
            Server.RegisterHandler(LoginEvent.LOGOUT_REQUEST, OnPlayerLogoutRequest);
            Server.RegisterHandler(LoginEvent.PLAYER_CARDS_REQUEST, OnPlayerCardsRequest);
            Server.RegisterHandler(LoginEvent.GAME_CARDS_REQUEST, OnGameCardsRequest);
        }

        /// <summary>
        /// Notifies a new player connection
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnPlayerConnection(NetworkMessage netMsg)
        {
            Debug.Log($"A player with id {netMsg.Sender.connectionId} has joined");
        }

        /// <summary>
        /// Manual Logout Handling
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnPlayerLogoutRequest(NetworkMessage netMsg)
        {
            if (netMsg.Sender.connectionId != null)
            {
                Debug.Log("Disconnecting user id = " + netMsg.Sender.connectionId);
                var disconnectedPlayerSession = SessionManager.Find(netMsg.Sender.connectionId);
                
                //Notify all friends he has
                if (disconnectedPlayerSession != null)
                {
                    var removedPlayerFriends = UserFriends.GetAll(disconnectedPlayerSession.Id);
                    List<NetworkClient> groupToSend = new List<NetworkClient>();

                    if (removedPlayerFriends.Count > 0)
                    {
                        foreach (var friend in removedPlayerFriends)
                        {
                            if (SessionManager.Exists(friend.Id))
                            {
                                var friendSession = SessionManager.Find(friend.Id);
                                if (friendSession != null)
                                    groupToSend.Add(friendSession.conn);
                            }
                        }
                    }

                    if (groupToSend.Count > 0)
                        Server.SendToGroup(groupToSend, LoginEvent.PLAYER_FRIENDS_LOGOUT, disconnectedPlayerSession.Id);
                    
                    SessionManager.Remove(disconnectedPlayerSession.Id);
                }
            }
        }
        /// <summary>
        /// Notifies and removes a player session
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnPlayerDisconnection(NetworkMessage netMsg)
        {
            Debug.Log($"A player with id {netMsg.Sender.connectionId} has disconnected from the Server");
            var disconnectedPlayerSession = SessionManager.Find(netMsg.Sender.connectionId);
            if (disconnectedPlayerSession != null)
            {
                var removedPlayerFriends = UserFriends.GetAll(disconnectedPlayerSession.Id);
                List<NetworkClient> groupToSend = new List<NetworkClient>();

                if (removedPlayerFriends.Count > 0)
                {
                    foreach (var friend in removedPlayerFriends)
                    {
                        if (SessionManager.Exists(friend.Id))
                        {
                            var friendSession = SessionManager.Find(friend.Id);
                            if (friendSession != null)
                                groupToSend.Add(friendSession.conn);
                        }
                    }
                }

                if (groupToSend.Count > 0)
                    Server.SendToGroup(groupToSend, LoginEvent.PLAYER_FRIENDS_LOGOUT, disconnectedPlayerSession.Id);
            }

            SessionManager.Remove(netMsg.Sender.connectionId);
        }

        /// <summary>
        /// Handler for new player authentication request
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnPlayerAuthRequest(NetworkMessage netMsg)
        {
            var authdata = netMsg.ReadMessage<AuthData>();

            var profileData = Users.Select(authdata.username, authdata.password);
            if (profileData != null)
            {
                if (SessionManager.Exists(profileData.Id))
                {
                    SessionManager.Remove(profileData.Id);
                    //Also kick the guy socket
                    Debug.Warning("Removed a session active with the account asking to Auth");
                }
                SessionManager.New(netMsg.Sender, profileData.Id);

                Server.SendToClient(netMsg.Sender, LoginEvent.LOGIN_SUCCESS, profileData);
                //Lets notify friends he has logged in
                List<NetworkClient> groupToSend = new List<NetworkClient>();
                var friends = UserFriends.GetAll(profileData.Id);
                if (friends.Count > 0)
                {
                    foreach(var friend in friends)
                    {
                        if (SessionManager.Exists(friend.Id))
                        {
                            var friendSession = SessionManager.Find(friend.Id);
                            if (friendSession != null)
                                groupToSend.Add(friendSession.conn);
                        }
                    }
                }

                if (groupToSend.Count > 0)
                    Server.SendToGroup(groupToSend, LoginEvent.PLAYER_FRIENDS_LOGIN, profileData.Id);
            }
            else
                Server.SendToClient(netMsg.Sender, LoginEvent.LOGIN_FAIL, new ActionError { code = 100, message = "User does not exist or password is wrong" });
        }

        /// <summary>
        /// Handler for player cards request
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnPlayerCardsRequest(NetworkMessage netMsg)
        {
            var userId = SessionManager.GetPlayerId(netMsg.Sender);
            var userDecks = PlayerDeckCards.GetPlayerDecks(userId);
            if (userDecks.Count > 0)
                Server.SendToClient(netMsg.Sender, LoginEvent.PLAYER_CARDS_SUCCESS, userDecks);
            else
                Server.SendToClient(netMsg.Sender, LoginEvent.PLAYER_CARDS_FAIL, null);
        }

        /// <summary>
        /// Handler for the game cards request, which means client
        /// will retrieve all the cards from the database
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnGameCardsRequest(NetworkMessage netMsg)
        {
            //We better have cards
            var gameCards = GameCards.GetAll();
            if (gameCards.Count > 0)
                Server.SendToClient(netMsg.Sender, LoginEvent.GAME_CARDS_SUCCESS, gameCards);
            else
                Server.SendToClient(netMsg.Sender, LoginEvent.GAME_CARDS_FAIL, null);
        }
    }
}
