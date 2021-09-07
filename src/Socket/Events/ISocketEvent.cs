using System;
using System.Linq;

using Fleck;

using Newtonsoft.Json.Linq;

using Cortex.Server.Socket.Clients;

namespace Cortex.Server.Socket.Events {
    interface ISocketEvent {
        string Event { get; }
        
        int Execute(SocketClient client, JToken data);   
    }
}
