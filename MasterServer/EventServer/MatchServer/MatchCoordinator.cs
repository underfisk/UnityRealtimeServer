using System;
using System.Collections.Generic;
using System.Text;
using Networking;
using Networking.Core;

namespace MasterServer.EventServer.MatchServer
{
    public class MatchCoordinator
    {
        private readonly NetworkServer _server;
        public MatchCoordinator(NetworkServer sv)
        {
            this._server = sv;

            this.Initialize();
        }

        /// <summary>
        /// Initializes the _server handlers
        /// </summary>
        private void Initialize()
        {
            _server.RegisterHandler(MatchEvent.TABLE_PLACE_CARD_REQUEST, OnPlayerPlaceCard);
        }

        /// <summary>
        /// Handles a client request to place a card
        /// whether it is a enemy or not, in _server doesn't matter
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnPlayerPlaceCard(NetworkMessage netMsg)
        {
            var playerId = netMsg.ReadMessage<uint>();
            
        }
        
    }
}
