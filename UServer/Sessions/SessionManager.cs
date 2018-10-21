using System;
using System.Collections.Generic;
using System.Text;
using UNetwork;

namespace UServer.Sessions
{
    /// <summary>
    /// SessionManager manages PlayerSessions with core actions to support with in-game data/server-data
    /// </summary>
    public static class SessionManager
    {
        private static List<PlayerSession> playerSessions = new List<PlayerSession>();

        /// <summary>
        /// Creates a new session for a given socket client and playerid(main key for operations)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="playerid"></param>
        public static void New (NetworkClient client, uint playerid)
        {
            playerSessions.Add(new PlayerSession(client, playerid));
            Debug.Log("New player session created : " + client.connectionId + " with db id = " + playerid);
        }

        /// <summary>
        /// Returns whether a given playerid already has a session
        /// </summary>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public static bool Exists(uint playerid)
        {
            return (playerSessions.FindAll(x => x.Id == playerid).Count > 0);
        }

        /// <summary>
        /// Returns whether a given connection exists
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public static bool Exists(string connectionId)
        {
            return (playerSessions.FindAll(x => x.conn.connectionId == connectionId).Count > 0);
        }

        public static void Find()
        {

        }

        public static void Update()
        {

        }
        
        /// <summary>
        /// Removes a player session with the connection id
        /// </summary>
        /// <param name="connectionId"></param>
        public static void Remove(string connectionId)
        {
            if (playerSessions.Count > 0)
                playerSessions.RemoveAll(x => x.conn.connectionId == connectionId);
        }

        /// <summary>
        /// Removes a player session with the given player id
        /// </summary>
        /// <param name="playerid"></param>
        public static void Remove(uint playerid)
        {
            if (playerSessions.Count > 0)
                playerSessions.RemoveAll(x => x.Id == playerid);
        }
    }
}
