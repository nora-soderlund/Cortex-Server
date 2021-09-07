using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Actions;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Rooms.Map.Events {
    class OnRoomMapStackUpdate : ISocketEvent {
        public string Event => "OnRoomMapStackUpdate";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            client.Send(new SocketMessage("OnRoomMapStackUpdate", client.User.Room.Map.GetStackableFloor()).Compose());

            return 1;
        }
    }
}
