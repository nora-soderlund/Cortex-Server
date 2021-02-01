using System;
using System.Linq;

using Fleck;

using Newtonsoft.Json;

using Server.Game.Users;

namespace Server.Socket.Clients {
    class SocketClient {
        public IWebSocketConnection Connection;

        public GameUser User;

        public SocketClient(IWebSocketConnection connection, GameUser user) {
            Connection = connection;
            User = user;
        }

        public void Send(string message) {
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
