using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Actions;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Rooms.Map.Events {
    class OnRoomMapStackUpdate : ISocketEvent {
        public string Event => "OnRoomMapStackUpdate";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            client.Send(new SocketMessage("OnRoomMapStackUpdate", client.User.Room.Map.GetStackMap()).Compose());

            return 1;
        }
    }
}
