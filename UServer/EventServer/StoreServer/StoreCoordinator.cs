using Networking.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityServer.EventServer.StoreServer
{
    /// <summary>
    /// This threaded server handles all the transactions/store events
    /// </summary>
    public sealed class StoreCoordinator
    {
        /// <summary>
        /// UNetwork Server Instance booted already in our main function
        /// </summary>
        private NetworkServer server;

        public StoreCoordinator(NetworkServer runningServer)
        {
            this.server = runningServer;
        }
    }
}
