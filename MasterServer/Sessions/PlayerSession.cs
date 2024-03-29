﻿using Networking.Sessions;
using System;
using MasterServer.Entity;

namespace MasterServer.Sessions
{
    public class PlayerSession
    {
        public DateTime LoggedTime { get; set; }
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
