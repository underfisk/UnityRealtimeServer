using System;
using System.Collections.Generic;
using System.Text;
using UNetwork;
using UServer.Database;
using UServer.Sessions;

namespace UServer.Coordinators
{
    /// <summary>
    /// This sub-server is responsible for handling general requests such as Friend Events, News Events, Update Events, Creation Events
    /// </summary>
    public sealed class GameServer
    {
        /// <summary>
        /// UNetwork Server Instance booted already in our main function
        /// </summary>
        private NetworkServer server;

        public GameServer(NetworkServer runningServer)
        {
            this.server = runningServer;
            Initialize();
        }

        /// <summary>
        /// Initializes all the handlers
        /// </summary>
        private void Initialize()
        {
            server.RegisterHandler(NetworkEvents.PLAYER_FRIENDS_REQUEST, OnPlayerFriendsRequest);
        }

        /// <summary>
        /// Handler responsable for retrieve player friends and return it
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnPlayerFriendsRequest(NetworkMessage netMsg)
        {
            var userId = SessionManager.GetPlayerId(netMsg.Sender);
            var friends = UserFriends.GetAll(userId);
            if (friends.Count > 0)
            {
                server.SendToClient(netMsg.Sender, NetworkEvents.PLAYER_FRIENDS_SUCCESS, friends);
            }
            else
            {
                Debug.Warning("He has none so just send a event error message type saying it");
            }
        }
    }
}
