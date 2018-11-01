using System;
using System.Collections.Generic;
using System.Text;

namespace MasterServer.EventServer.GameServer
{
    public static class GameEvent
    {
        //Opcodes for player friends with events and request based
        public static readonly short PLAYER_FRIENDS_REQUEST = 1200;
        public static readonly short PLAYER_FRIENDS_SUCCESS = 1201;
        public static readonly short PLAYER_FRIENDS_FAIL = 1202;
        public static readonly short PLAYER_FRIENDS_NEW = 1203;
        public static readonly short PLAYER_FRIENDS_DELETE = 1204;
        public static readonly short PLAYER_FRIENDS_ON_NEW = 1205;
        public static readonly short PLAYER_FRIENDS_LOGIN = 1206;
        public static readonly short PLAYER_FRIENDS_LOGOUT = 1208;
    }
}
