using Networking;
using Networking.Core;
using Networking.Sessions;
using Networking.Utility;
using MasterServer.Database;
using MasterServer.Sessions;

namespace MasterServer.EventServer.GameServer
{
    /// <summary>
    /// This sub-_server is responsible for handling general requests such as Friend Events, News Events, Update Events, Creation Events
    /// </summary>
    public sealed class GameCoordinator
    {
        /// <summary>
        /// UNetwork _server Instance booted already in our main function
        /// </summary>
        private readonly NetworkServer _server;

        public GameCoordinator(NetworkServer runningServer)
        {
            this._server = runningServer;
            Initialize();
        }

        /// <summary>
        /// Initializes all the handlers
        /// </summary>
        private void Initialize()
        {
            _server.RegisterHandler(GameEvent.PLAYER_FRIENDS_REQUEST, OnPlayerFriendsRequest);
        }

        /// <summary>
        /// Handler a request to0 retrieve aa player friends
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnPlayerFriendsRequest(NetworkMessage netMsg)
        {
            var userId = SessionManager.GetPlayerId(netMsg.Sender);
            var friends = UserFriends.GetAll(userId);
            if (friends.Count > 0)
                _server.SendToClient(netMsg.Sender, GameEvent.PLAYER_FRIENDS_SUCCESS, friends);
            else
                Debug.Warning("He has none so just send a event error message type saying it");
        }
    }
}
