using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace UNetwork
{
    /// <summary>
    /// NetworkServer is the main network operations coordinator which is responsible to hold
    /// some threads related with Networking Acknowledgement
    /// @TODO: Make some modules to be easier to work with this
    /// </summary>
    public sealed class NetworkServer : IDisposable
    {
        private TcpListener _listener = null;
        private IPAddress _addr = null;
        private Int32 _port = 0;
        private Thread _heartbeat = null;
        private List<NetworkClient> _connectedSockets = new List<NetworkClient>();
        private bool serverIsRunning = false;

        /// <summary>
        /// Thread signal
        /// </summary>
        private ManualResetEvent allDone = new ManualResetEvent(false);

        /// <summary>
        /// Delegate abstraction for communication over callbacks
        /// </summary>
        /// <param name="netMsg"></param>
        public delegate void NetworkMessageDelegate(NetworkMessage netMsg);

        /// <summary>
        /// Registered Handlers map 
        /// </summary>
        private List<Dictionary<short, NetworkMessageDelegate>> handlersMap = new List<Dictionary<short, NetworkMessageDelegate>>();

        /// <summary>
        /// Delegate abstraction for internal disconnect
        /// </summary>
        /// <param name="netMsg"></param>
        public delegate void OnDisconnect(NetworkMessage netMsg);

        /// <summary>
        /// Event for internal disconnect
        /// </summary>
        private event OnDisconnect OnNetworkClientDisconnect;


        /// <summary>
        /// Starts a new listener with the provided address and port
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="port"></param>
        public NetworkServer(string addr, Int32 port)
        {
            this._addr = IPAddress.Parse(addr);
            this._port = port;
            this._listener = new TcpListener(this._addr, this._port);
        }

        /// <summary>
        /// Starts a new listener with the provided IPAdress object and port
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="port"></param>
        public NetworkServer(IPAddress addr, Int32 port)
        {
            this._addr = addr;
            this._port = port;
            this._listener = new TcpListener(this._addr, this._port);
        }

        /// <summary>
        /// Registers a new handler callback for event based system
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="handler"></param>
        public void RegisterHandler(short opcode, NetworkMessageDelegate handler)
        {
            var op = new Dictionary<short, NetworkMessageDelegate>();
            op.Add(opcode, handler);
            this.handlersMap.Add(op);
        }

        /// <summary>
        /// Deletes every delegation for this opcode, assuming you have knowledge of this
        /// proceed carefully (This function is mentioned to be likely a Reset for the opcode)
        /// </summary>
        /// <param name="opcode"></param>
        public void UnregisterHandler(short opcode)
        {
            this.handlersMap.ForEach((item) =>
            {
                if (item.ContainsKey(opcode))
                    item.Remove(opcode);
            });
        }

        /// <summary>
        /// Starts the Listening for TCP/IP Sockets using heartbeat with an interval to detect
        /// disconnections
        /// </summary>
        public void Start()
        {
            try
            {
                //Starts hearing for clients
                this._listener.Start();

                //Register internal handlers
                OnNetworkClientDisconnect += OnClientDisconnect;

                //Define the server has already started for main loop
                this.serverIsRunning = true;

                //Starts heartbeat
                this.StartHeartbeat();

                //this var is stopped with Stop Function
                while (serverIsRunning)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    //Create a thread to accept
                    this._listener.BeginAcceptSocket(new AsyncCallback(OnNewSocket), null);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Sends data to a specific socket
        /// </summary>
        /// <param name="client"></param>
        /// <param name="opcode"></param>
        /// <param name="obj"></param>
        public void SendToClient(NetworkClient client, short opcode, object obj)
        {
            if (!client.IsConnected()) return;

            //Create a internal data structure
            var packet = new Packet
            {
                opcode = opcode,
                data = JPacketBuilder.Serialize(obj)
            };

            //Serialize all the packet data
            var payload = JPacketBuilder.Serialize(packet);

            //Now convert to a buffer byte 
            byte[] byteData = Encoding.ASCII.GetBytes(payload);

            Console.WriteLine($"Sending {byteData.Length} bytes to the client id " + client.connectionId);

            // Begin sending the data to the remote device.  
            client.Socket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        /// <summary>
        /// Sends data to all the connected sockets
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="obj"></param>
        public void SendToAll(short opcode, object obj)
        {
            for(var i = 0; i < this._connectedSockets.Count; i++)
            {
                var user = this._connectedSockets[i];
                if (user.IsConnected())
                {
                    //Create a internal data structure
                    var packet = new Packet
                    {
                        opcode = opcode,
                        data = JPacketBuilder.Serialize(obj)
                    };

                    //Serialize all the packet data
                    var payload = JPacketBuilder.Serialize(packet);

                    //Now convert to a buffer byte 
                    byte[] byteData = Encoding.ASCII.GetBytes(payload);

                    Console.WriteLine($"Sending {byteData.Length} bytes to the client id " + user.connectionId);

                    // Begin sending the data to the remote device.  
                    user.Socket.BeginSend(byteData, 0, byteData.Length, 0,
                        new AsyncCallback(SendCallback), user);
                }
            }
        }

        /// <summary>
        /// Sends data to a specific group of connected users
        /// </summary>
        /// <param name="group"></param>
        /// <param name="opcode"></param>
        /// <param name="obj"></param>
        public void SendToGroup(List<NetworkClient> clients, short opcode, object obj)
        {
            if (clients.Count > 0)
            {
                lock(clients)
                {
                    foreach(var client in clients)
                    {
                        if (client.IsConnected())
                        {
                            //Create a internal data structure
                            var packet = new Packet
                            {
                                opcode = opcode,
                                data = JPacketBuilder.Serialize(obj)
                            };

                            //Serialize all the packet data
                            var payload = JPacketBuilder.Serialize(packet);

                            //Now convert to a buffer byte 
                            byte[] byteData = Encoding.ASCII.GetBytes(payload);

                            Console.WriteLine($"Sending {byteData.Length} bytes to the client id " + client.connectionId);

                            // Begin sending the data to the remote device.  
                            client.Socket.BeginSend(byteData, 0, byteData.Length, 0,
                                new AsyncCallback(SendCallback), client);
                        }
                    }
    
                }
            }
        }

        /// <summary>
        /// Define the keep alive time and interval
        /// </summary>
        /// <param name="on"></param>
        /// <param name="keepAliveTime"></param>
        /// <param name="keepAliveInterval"></param>
        public void SetKeepAlive(ref NetworkClient client, bool on, uint keepAliveTime, uint keepAliveInterval)
        {
            int size = Marshal.SizeOf(new uint());

            var inOptionValues = new byte[size * 3];

            BitConverter.GetBytes((uint)(on ? 1 : 0)).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)keepAliveTime).CopyTo(inOptionValues, size);
            BitConverter.GetBytes((uint)keepAliveInterval).CopyTo(inOptionValues, size * 2);

            client.Socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }

        /// <summary>
        /// Callback when a new client joins
        /// </summary>
        /// <param name="ar"></param>
        private void OnNewSocket(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();
            var handler = new NetworkClient(this._listener.EndAcceptSocket(ar));
            handler.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            handler.Socket.BeginReceive(handler.buffer, 0, handler.buffer.Length, SocketFlags.None, new AsyncCallback(OnClientMessage), handler);

            NotifyNewConnection(new NetworkMessage(handler, null));
            this._connectedSockets.Add(handler);
            this._listener.BeginAcceptSocket(OnNewSocket, null);

            
        }

        /// <summary>
        /// Subscribed event for disconnection detect
        /// </summary>
        /// <param name="client"></param>
        private void OnClientDisconnect(NetworkMessage netMsg)
        {
            netMsg.Sender.Socket.Close();
            this._connectedSockets.Remove(netMsg.Sender);
        }

        /// <summary>
        /// This is mentioned to be a core for when a new message arrives from a client
        /// but make a simplier version where we read an opcode, data and send in a structured data
        /// the socket who's doing this
        /// </summary>
        /// <param name="ar"></param>
        private void OnClientMessage(IAsyncResult ar)
        {
            var handler = (NetworkClient)ar.AsyncState;

            if (handler == null)
            {
                Debug.Warning("We got a null handler at OnClientMessage. Handle this!");
            }
            try
            {
                SocketError error;
                int read = handler.Socket.EndReceive(ar, out error);
                if (error != SocketError.Success)
                {
                    //This means a socket has disconnected
                    if (error == SocketError.ConnectionReset)
                    {
                        var netMsg = new NetworkMessage(handler, null);
                        NotifyDisconnection(netMsg);
                        return;
                    }
                }
                else if (error == SocketError.Success)
                {
                    if (read > 0)
                    {
                        //Extract json data
                        var encodedBytes = Encoding.UTF8.GetString(handler.buffer);
                        var packet = JPacketBuilder.Deserialize<Packet>(encodedBytes);
                        var msg = new NetworkMessage(handler, packet.data);

                        //Notify the callbacks waiting for this opcode
                        NotifyDataReceived(packet.opcode, msg);

                        //Clear the buffer
                        handler.ClearBuffer();

                        //Return getting more
                        handler.Socket.BeginReceive(handler.buffer, 0, handler.buffer.Length, 0, new AsyncCallback(OnClientMessage), handler);
                    }
                    else
                    {
                        if (handler.IsConnected())
                            handler.Socket.Disconnect(false);

                        var netMsg = new NetworkMessage(handler, null);
                        NotifyDisconnection(netMsg);
                    }
                }
            }
            catch (Exception e) {
                // Handle all other exceptions
                Console.WriteLine(e.ToString());
                if (handler.IsConnected())
                    handler.Socket.Disconnect(false);

                var netMsg = new NetworkMessage(handler, null);
                NotifyDisconnection(netMsg);
            }

        }

        /// <summary>
        /// Callback for when we send a packet being notified
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                var handler = (NetworkClient)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.Socket.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        /// <summary>
        /// Notify all actions subscribed for this opcode
        /// </summary>
        /// <param name="netMsg"></param>
        private void NotifyDisconnection(NetworkMessage netMsg)
        {
            //Used for internal/external on this specific delegate
            if (OnNetworkClientDisconnect.GetInvocationList().Length > 0)
                OnNetworkClientDisconnect.Invoke(netMsg);

            //Support additional subscriptions
            this.handlersMap.ForEach((item) =>
            {
                if (item.ContainsKey(MsgType.Disconnect))
                    item[MsgType.Disconnect].Invoke(netMsg);
            });
        }

        /// <summary>
        /// Notify all actions subscribed about this opcode and give them the data
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="netMsg"></param>
        private void NotifyDataReceived(short opcode, NetworkMessage netMsg)
        {
            this.handlersMap.ForEach((item) =>
            {
                if (item.ContainsKey(opcode))
                    item[opcode].Invoke(netMsg);
            });
        }

        /// <summary>
        /// Notify all actions subscribed for connect opcode
        /// </summary>
        /// <param name="netMsg"></param>
        private void NotifyNewConnection(NetworkMessage netMsg)
        {
            this.handlersMap.ForEach((item) =>
            {
                if (item.ContainsKey(MsgType.Connect))
                    item[MsgType.Connect].Invoke(netMsg);
            });
        }

        /// <summary>
        /// Heartbeat detection for disconnections
        /// </summary>
        private void StartHeartbeat()
        {
            if (!serverIsRunning) return;

            this._heartbeat = new Thread(delegate ()
            {
                var nextTime = DateTime.Now.AddSeconds(1);
                while (this.serverIsRunning)
                {
                    if (nextTime < DateTime.Now && this._connectedSockets.Count > 0)
                    {
                        try
                        {
                            for (int i = 0; i < this._connectedSockets.Count; i++)
                            {
                                var client = this._connectedSockets[i];
                                if (client.Socket != null && !client.IsConnected())
                                {
                                    var netMsg = new NetworkMessage(client, null);
                                    NotifyDisconnection(netMsg);
                                }
                            }
                        }
                        catch (SocketException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        finally
                        {
                            nextTime = DateTime.Now.AddSeconds(2);
                        }
                    }
                }
            });

            //Starts
            this._heartbeat.Start();
        }

        /// <summary>
        /// Stops the server and when this action is complete
        /// it's recommneded to dispose
        /// </summary>
        public void Stop()
        {
            this._listener.Stop();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose managed/unmanaged resources used by this class
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._listener = null;
                    this._connectedSockets = null;
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Destructor called when we supress this class
        /// </summary>
        ~NetworkServer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the class
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
