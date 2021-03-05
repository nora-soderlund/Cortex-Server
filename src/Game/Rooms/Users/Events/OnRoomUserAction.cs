using System;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Users.Actions;

using Server.Socket.Clients;
using Server.Socket.Messages;
using Server.Socket.Events;

namespace Server.Game.Rooms.Users.Events {
    class OnRoomUserAction : ISocketEvent {
        public string Event => "OnRoomUserAction";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            string action = data.ToString();

            //if(action != "Wave")
            //    return 0;

            GameRoomUser roomUser = client.User.Room.GetUser(client.User.Id);

            if(roomUser.Actions.Contains(action))
                return 0;

            client.Send(new SocketMessage("OnRoomUserAction", true).Compose());

            client.User.Room.Actions.AddEntity(roomUser.Id, 500, new GameRoomUserAction(roomUser, action, 2000));

            return 1;
        }
    }
}
