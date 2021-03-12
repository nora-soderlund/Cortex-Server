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

namespace Server.Game.Rooms.Chat.Events {
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
