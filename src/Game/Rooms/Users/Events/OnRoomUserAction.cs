using System;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Users.Actions;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Messages;
using Cortex.Server.Socket.Events;

namespace Cortex.Server.Game.Rooms.Users.Events {
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

            client.User.Room.Actions.AddEntity(roomUser.Id, new GameRoomUserAction(roomUser, action, 2000));

            return 1;
        }
    }
}
