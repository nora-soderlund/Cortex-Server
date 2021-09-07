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

namespace Cortex.Server.Game.Rooms.Chat.Events {
    class OnRoomUserChat : ISocketEvent {
        public string Event => "OnRoomUserChat";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            string message = data.ToString();

            if(message.Length == 0)
                return 0;

            GameRoomChat.Call(client.User.Room.GetUser(client.User.Id), message);

            return 1;
        }
    }
}
