using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Furnitures;

using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Actions;
using Cortex.Server.Game.Rooms.Furnitures.Actions;

using Cortex.Server.Game.Furnitures;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Rooms.Furnitures.Events {
    class OnRoomFurnitureRotate : ISocketEvent {
        public string Event => "OnRoomFurnitureRotate";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            GameRoomUser roomUser = client.User.Room.GetUser(client.User.Id);

            if(!roomUser.HasRights())
                return 0;
            
            int id = data["id"].ToObject<int>();
            
            GameRoomFurniture roomFurniture = client.User.Room.Furnitures.Find(x => x.Id == id);

            if(roomFurniture == null)
                return 0;

            
            int direction = data["direction"].ToObject<int>();

            client.Send(new SocketMessage("OnRoomFurnitureRotate", roomFurniture.Id).Compose());

            client.User.Room.Actions.AddEntity(roomFurniture.Id, new GameRoomFurniturePosition(roomFurniture, new GameRoomPoint(roomFurniture.Position.Row, roomFurniture.Position.Column, roomFurniture.Position.Depth, direction)));

            return 1;
        }
    }
}
