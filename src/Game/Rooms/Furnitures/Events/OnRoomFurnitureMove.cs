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
    class OnRoomFurnitureMove : ISocketEvent {
        public string Event => "OnRoomFurnitureMove";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;
            
            int id = data["id"].ToObject<int>();
            
            GameRoomFurniture roomFurniture = client.User.Room.Furnitures.Find(x => x.Id == id);

            if(roomFurniture == null)
                return 0;

            int row = data["position"]["row"].ToObject<int>();
            int column = data["position"]["column"].ToObject<int>();
            
            int direction = data["position"]["direction"].ToObject<int>();

            if(row == roomFurniture.Position.Row && column == roomFurniture.Position.Column && direction == roomFurniture.Position.Direction)
                return 0;

            if(!client.User.Room.Map.IsValidFloor(row, column))
                return 0;

            GameRoomFurniture stackedFurniture = client.User.Room.Map.GetFloorFurniture(row, column);

            double depth = client.User.Room.Map.GetFloorDepth(row, column);

            if(stackedFurniture != null) {
                if(!stackedFurniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Stackable))
                    return 0;

                depth = stackedFurniture.Position.Depth + stackedFurniture.GetDimension().Depth;
            }

            client.Send(new SocketMessage("OnRoomFurnitureMove", roomFurniture.Id).Compose());

            client.User.Room.Actions.Add(500, "OnRoomEntityUpdate", "furnitures", new GameRoomFurniturePosition(roomFurniture, new GameRoomPoint(row, column, depth, direction)));

            return 1;
        }
    }
}
