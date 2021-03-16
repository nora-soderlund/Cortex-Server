using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Users.Actions;
using Server.Game.Rooms.Actions;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Rooms.Map.Events {
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
