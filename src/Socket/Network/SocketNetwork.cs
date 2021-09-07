using System;
using System.Linq;

using Newtonsoft.Json.Linq;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Messages;
using Cortex.Server.Socket.Events;

namespace Cortex.Server.Socket.Network {
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
