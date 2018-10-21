using System;
using System.Net.Sockets;

namespace UNetwork
{
    /// <summary>
    /// NetworkClient is an class which holds a NetworkConnnection and related information about the other side
    /// who's connected to the server.
    /// </summary>
    public class NetworkClient
    {


        /// <summary>
        /// Unique ID generated based on Guid
        /// </summary>
        private string id;

        /// <summary>
        /// Max size of data buffering
        /// </summary>
        public int bufferSize = 1024;

        /// <summary>
        /// Buffer to workout with the data received
        /// </summary>
        public byte[] buffer;

        /// <summary>
        /// Socket instance
        /// </summary>
        public Socket Socket;

        /// <summary>
        /// Connection id is an alias for the unique generated ID
        /// </summary>
        public string connectionId { get { return this.id; }  }

        /// <summary>
        /// Verifies whether the user is connected
        /// On server side this is much more relilable than using socket.Connected
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            try
            {
                return !(this.Socket.Poll(1, SelectMode.SelectRead) && this.Socket.Available == 0);
            }
            catch (SocketException) { return false; }
            catch (ObjectDisposedException) { return false; }
        }

        /// <summary>
        /// Creates a default instance (Mostly used for async operations to hold a temporary instance)
        /// </summary>
        public NetworkClient()
        {
            buffer = new byte[bufferSize];
            id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        /// <summary>
        /// Creates an instance with a given socketInstance
        /// </summary>
        /// <param name="socketInstance"></param>
        public NetworkClient(Socket socketInstance)
        {
            this.Socket = socketInstance;
            buffer = new byte[bufferSize];
            id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }     

        /// <summary>
        /// Clear the data buffer to prevent read errors
        /// </summary>
        public void ClearBuffer()
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
        }
    }
}
