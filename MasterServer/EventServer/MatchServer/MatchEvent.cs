namespace MasterServer.EventServer.MatchServer
{
    public struct MatchEvent
    {
        public static readonly short TABLE_PLACE_CARD_REQUEST = 5000,
            TABLE_PLACE_CARD_SUCCESS = 5001,
            TABLE_PLACE_CARD_FAIL = 5002,
            TABLE_PLACE_CARD_NOTIFY = 5003, //notifies the enemy player
            TABLE_ATTACK_CARD_REQUEST = 5004;
    }
}