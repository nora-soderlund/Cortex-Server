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
    class OnRoomUserChat : ISocketEvent {
        public string Event => "OnRoomUserChat";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            string message = data.ToString();

            client.User.Room.Send(new SocketMessage("OnRoomUserChat", new {
                id = client.User.Id,

                message = message
            }).Compose());

            return 1;
        }
    }
}
