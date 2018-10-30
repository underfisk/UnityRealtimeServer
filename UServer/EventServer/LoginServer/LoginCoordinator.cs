using Networking;
using Networking.Core;
using Networking.Sessions;
using Networking.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using UnityServer;
using UnityServer.Database;
using UnityServer.Sessions;

namespace UnityServer.EventServer.LoginServer
{
    /// <summary>
    /// This threaded server handles all the login/session events
    /// </summary>
    public sealed class LoginCoordinator
    {
        /// <summary>
        /// UNetwork Server Instance booted already in our main function
        /// </summary>
        private NetworkServer server;

        public LoginCoordinator(NetworkServer runningServer)
        {
            this.server = runningServer;
            Initialize();
        }

        /// <summary>
        /// Initializes the server callsback
        /// </summary>
        private void Initialize()
        {
            server.RegisterHandler(MsgType.Connect, OnPlayerConnection);
            server.RegisterHandler(MsgType.Disconnect, OnPlayerDisconnection);
            server.RegisterHandler(LoginEvent.LOGIN_REQUEST, OnPlayerAuthRequest);
            server.RegisterHandler(LoginEvent.PLAYER_CARDS_REQUEST, OnPlayerCardsRequest);
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
        /// Notifies and removes a player session
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnPlayerDisconnection(NetworkMessage netMsg)
        {
            Debug.Log($"A player with id {netMsg.Sender.connectionId} has disconnected from the server");
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
                    server.SendToGroup(groupToSend, LoginEvent.PLAYER_FRIENDS_LOGOUT, disconnectedPlayerSession.Id);
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

                server.SendToClient(netMsg.Sender, LoginEvent.LOGIN_SUCCESS, profileData);
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
                    server.SendToGroup(groupToSend, LoginEvent.PLAYER_FRIENDS_LOGIN, profileData.Id);
            }
            else
                server.SendToClient(netMsg.Sender, LoginEvent.LOGIN_FAIL, new ActionError { code = 100, message = "User does not exist or password is wrong" });
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
            {
                server.SendToClient(netMsg.Sender, LoginEvent.PLAYER_CARDS_SUCCESS, userDecks);
            }
            else
            {
                Debug.Warning("Notify him soon");
                server.SendToClient(netMsg.Sender, LoginEvent.PLAYER_CARDS_FAIL, null);
            }
        }
    }
}
