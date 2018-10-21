
namespace UNetwork
{
    /// <summary>
    /// Internal opcodes for predefined network actions with NetworkClients
    /// </summary>
    public class MsgType
    {
        /// <summary>
        /// Internal networking system message for communicating a connection has occurred.
        /// </summary>
        public static readonly short Connect = 0x100;

        /// <summary>
        /// Internal networking system message for communicating a disconnect has occurred.
        /// </summary>
        public static readonly short Disconnect = 0x200;


    }
}
