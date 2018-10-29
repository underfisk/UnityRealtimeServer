using System;
using System.Collections.Generic;
using System.Text;
using UNetwork;
using UServer.Database;

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

        /// <summary>
        /// Returns the player id associated with the socket
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static uint GetPlayerId(NetworkClient client)
        {
            foreach(var sess in playerSessions)
            {
                if (sess.conn.connectionId == client.connectionId)
                    return sess.Id;
            }

            return 0;
        }

        public static PlayerSession Find(string connId)
        {
            return playerSessions.Find(x => x.conn.connectionId == connId);
        }

        public static PlayerSession Find(uint playerid)
        {
            return playerSessions.Find(x => x.Id == playerid);
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
