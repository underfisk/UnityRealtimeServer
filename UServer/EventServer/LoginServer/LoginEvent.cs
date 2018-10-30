using System;
using System.Collections.Generic;
using System.Text;

namespace UnityServer.EventServer.LoginServer
{
    public static class LoginEvent
    {
        //Opcodes for login process
        public static readonly short LOGIN_REQUEST = 1000;
        public static readonly short LOGIN_SUCCESS = 1001;
        public static readonly short LOGIN_FAIL = 1002;

        //Opcodes for after login success to request cards data
        public static readonly short PLAYER_CARDS_REQUEST = 1100;
        public static readonly short PLAYER_CARDS_SUCCESS = 1101;
        public static readonly short PLAYER_CARDS_FAIL = 1102;

        //Opcode for manual logout request
        public static readonly short LOGOUT_REQUEST = 1003;

        //Shared opcodes
        public static readonly short PLAYER_FRIENDS_LOGIN = 1206;
        public static readonly short PLAYER_FRIENDS_LOGOUT = 1208;
    }
}
