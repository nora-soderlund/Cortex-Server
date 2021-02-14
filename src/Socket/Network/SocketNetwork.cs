using System;
using System.Linq;

using Newtonsoft.Json.Linq;

using Server.Socket.Clients;
using Server.Socket.Messages;
using Server.Socket.Events;

namespace Server.Socket.Network {
    class OnSocketPing : ISocketEvent {
        public string Event => "OnSocketPing";

        public int Execute(SocketClient client, JToken data) {
            long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            client.Send(new SocketMessage("OnSocketPing", new {
                time, received = client.Received, sent = client.Sent
            }).Compose());

            return 1;
        }
    }
}
