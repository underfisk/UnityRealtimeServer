using Networking;
using Networking.Core;
using Networking.Sessions;
using Networking.Utility;
using MasterServer.Database;
using MasterServer.Sessions;

namespace MasterServer.EventServer.GameServer
{
    /// <summary>
    /// This sub-Server is responsible for handling general requests such as Friend Events, News Events, Update Events, Creation Events
    /// </summary>
    public sealed class GameCoordinator
    {
        /// <summary>
        /// UNetwork Server Instance booted already in our main function
        /// </summary>
        private NetworkServer Server;

        public GameCoordinator(NetworkServer runningServer)
        {
            this.Server = runningServer;
            Initialize();
        }

        /// <summary>
        /// Initializes all the handlers
        /// </summary>
        private void Initialize()
        {
            Server.RegisterHandler(GameEvent.PLAYER_FRIENDS_REQUEST, OnPlayerFriendsRequest);
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
                Server.SendToClient(netMsg.Sender, GameEvent.PLAYER_FRIENDS_SUCCESS, friends);
            else
                Debug.Warning("He has none so just send a event error message type saying it");
        }
    }
}
