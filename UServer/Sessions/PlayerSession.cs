using System;
using UNetwork;
using UServer.GameEntity;

namespace UServer.Sessions
{
    public class PlayerSession
    {
        public DateTime LoggedTime { get; set; }
        public Player Entity { get; set; }
        public NetworkClient conn { get; }
        public bool IsLoggedIn { get; set; }
        public uint Id { get; set; }

        public PlayerSession(NetworkClient _client, uint playerid)
        {
            conn = _client;
            Id = playerid;
            LoggedTime = DateTime.UtcNow;
        }
    }
}
