using System;

namespace UNetwork
{
    public class NetworkMessage
    {
        /// <summary>
        /// NetworkClient who has sent it
        /// </summary>
        public NetworkClient Sender { get; set; }

        /// <summary>
        /// JSON serialized data
        /// </summary>
        private string jdata;

        /// <summary>
        /// Creates a new network message to travel with the bounder who has created it
        /// </summary>
        /// <param name="_conn"></param>
        /// <param name="_jdata"></param>
        public NetworkMessage(NetworkClient _conn, string _jdata)
        {
            this.Sender = _conn;
            this.jdata = _jdata;
        }

        /// <summary>
        /// Reads a serialized stream message
        /// </summary>
        /// <typeparam name="TMsg"></typeparam>
        /// <returns></returns>
        public TMsg ReadMessage<TMsg>()
        {
            //Since we are dealing with json packets lets be sure
            //we saved a json string
            if (this.jdata.GetType() != typeof(String))
                return default(TMsg);
            
            //Is the string ok?
            if (!String.IsNullOrWhiteSpace(this.jdata))
                return JPacketBuilder.Deserialize<TMsg>(this.jdata);
            else
                return default(TMsg);
        }
    }
}
