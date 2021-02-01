using System;
using System.Linq;

using Fleck;

using Newtonsoft.Json.Linq;

using Server.Socket.Clients;

namespace Server.Socket.Events {
    interface ISocketEvent {
        string Event { get; }
        
        int Execute(SocketClient client, JToken data);   
    }
}
