
namespace UServer
{
    public static class NetworkEvents
    {

        //Opcodes for login process
        public static readonly short LOGIN_REQUEST = 1000;
        public static readonly short LOGIN_SUCCESS = 1001;
        public static readonly short LOGIN_FAIL = 1002;

        //Opcodes for after login success to request cards data
        public static readonly short PLAYER_CARDS_REQUEST = 1100;
        public static readonly short PLAYER_CARDS_SUCCESS = 1101;
        public static readonly short PLAYER_CARDS_FAIL = 1102;

        //Opcodes for player friends with events and request based
        public static readonly short PLAYER_FRIENDS_REQUEST = 1200;
        public static readonly short PLAYER_FRIENDS_SUCCESS = 1201;
        public static readonly short PLAYER_FRIENDS_FAIL = 1202;
        public static readonly short PLAYER_FRIENDS_NEW = 1203;
        public static readonly short PLAYER_FRIENDS_DELETE = 1204;
        public static readonly short PLAYER_FRIENDS_ON_NEW = 1205;
        public static readonly short PLAYER_FRIENDS_LOGIN = 1206;
        public static readonly short PLAYER_FRIENDS_LOGOUT = 1208;

        //Opcode for manual logout request
        public static readonly short LOGOUT_REQUEST = 1003;

    }
}
