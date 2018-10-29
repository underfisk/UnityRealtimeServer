using System;
using System.Collections.Generic;
using System.Text;
using UNetwork;

namespace UServer.Coordinators
{
    /// <summary>
    /// This threaded server handles all the transactions/store events
    /// </summary>
    public sealed class StoreServer
    {
        /// <summary>
        /// UNetwork Server Instance booted already in our main function
        /// </summary>
        private NetworkServer server;

        public StoreServer(NetworkServer runningServer)
        {
            this.server = runningServer;
        }
    }
}
