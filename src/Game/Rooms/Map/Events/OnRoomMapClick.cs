using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Users.Actions;
using Cortex.Server.Game.Rooms.Actions;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Rooms.Map.Events {
    class OnRoomMapClick : ISocketEvent {
        public string Event => "OnRoomMapClick";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            GameRoomUser roomUser = client.User.Room.Users.Find(x => x.User == client.User);
                
            int row = data.Value<int>("row");
            int column = data.Value<int>("column");

            //client.User.Room.Events.User[roomUser] = new GameRoomUserEvent(roomUser, row, column);

            //client.User.Room.Events.AddUser(roomUser, new GameRoomUserPositionAction(roomUser, row, column));
            client.User.Room.Actions.AddEntity(roomUser.Id, new GameRoomUserPosition(roomUser, row, column, 500));

            return 1;
        }
    }
}
