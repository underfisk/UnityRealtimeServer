using System;

namespace UNetwork
{
    [Serializable]
    /// <summary>
    /// Base Packet Structure
    /// TODO: Make this secure with the JSON.net encryptator
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// Identifier code
        /// </summary>
        public short opcode;

        /// <summary>
        /// JSON data as String to convert further for object
        /// </summary>
        public string data;

    }
}
