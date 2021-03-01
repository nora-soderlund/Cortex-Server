using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Users.Furnitures;

using Server.Game.Rooms;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Actions;
using Server.Game.Rooms.Furnitures.Actions;

using Server.Game.Furnitures;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Rooms.Furnitures.Events {
    class OnRoomFurnitureUse : ISocketEvent {
        public string Event => "OnRoomFurnitureUse";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;
            
            int id = data["id"].ToObject<int>();

            GameRoomFurniture roomFurniture = client.User.Room.Furnitures.Find(x => x.Id == id);

            if(roomFurniture == null)
                return 0;
                
            int animation = data["animation"].ToObject<int>();

            roomFurniture.Animation = animation;
            
            client.Send(new SocketMessage("OnRoomFurnitureUse", roomFurniture.Id).Compose());

            client.User.Room.Actions.Add(500, "OnRoomEntityUpdate", "furnitures", new GameRoomFurnitureAnimation(roomFurniture, animation));

            return 1;
        }
    }
}
