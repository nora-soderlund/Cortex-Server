using System;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Rooms;

using Server.Socket.Clients;
using Server.Socket.Events;

namespace Server.Game.Rooms.Users.Events {
    class OnSocketClose : ISocketEvent {
        public string Event => "OnSocketClose";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            GameRoomManager.RemoveUser(client.User);

            return 1;
        }
    }
}
