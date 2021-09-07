using System;
using System.Linq;

using Fleck;

using Newtonsoft.Json;

using Cortex.Server.Game.Users;

namespace Cortex.Server.Socket.Clients {
    class SocketClient {
        public IWebSocketConnection Connection;

        public GameUser User;

        public int Sent = 0;
        public int Received = 0;

        public SocketClient(IWebSocketConnection connection, GameUser user) {
            Connection = connection;
            User = user;
        }

        public void Send(string message) {
            Sent++;
            
            Connection.Send(message);
        }

        public string GetAddress() {
            return Connection.ConnectionInfo.ClientIpAddress;
        }

        public int GetPort() {
            return Connection.ConnectionInfo.ClientPort;
        }

        public string GetAddressPort() {
            return GetAddress() + ":" + GetPort();
        }
    }
}
