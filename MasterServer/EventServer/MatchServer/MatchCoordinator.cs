using System;
using System.Collections.Generic;
using System.Text;
using Networking;
using Networking.Core;

namespace MasterServer.EventServer.MatchServer
{
    public class MatchCoordinator
    {
        private NetworkServer server;
        public MatchCoordinator(NetworkServer sv)
        {
            this.server = sv;

            this.Initialize();
        }

        /// <summary>
        /// Initializes the server handlers
        /// </summary>
        private void Initialize()
        {
            server.RegisterHandler(MatchEvent.TABLE_PLACE_CARD_REQUEST, OnPlayerPlaceCard);
        }

        /// <summary>
        /// Handles a client request to place a card
        /// whether it is a enemy or not, in server doesn't matter
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnPlayerPlaceCard(NetworkMessage netMsg)
        {
            var playerId = netMsg.ReadMessage<uint>();
            
        }
    }
}
