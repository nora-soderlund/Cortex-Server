using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Users.Furnitures;

using Server.Game.Rooms;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Actions;

using Server.Game.Furnitures;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Rooms.Furnitures.Events {
    class OnRoomFurniturePlace : ISocketEvent {
        public string Event => "OnRoomFurniturePlace";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;
            
            string id = data["id"].ToString();

            GameUserFurniture userFurniture = client.User.Furnitures.Find(x => x.Furniture.Id == id && x.Room == 0);

            if(userFurniture == null)
                return 0;

            int row = data["position"]["row"].ToObject<int>();
            int column = data["position"]["column"].ToObject<int>();
            
            int direction = data["position"]["direction"].ToObject<int>();

            if(!client.User.Room.Map.IsValidFloor(row, column))
                return 0;

            GameRoomFurniture stackedFurniture = client.User.Room.Map.GetFloorFurniture(row, column);
            

            double depth = client.User.Room.Map.GetFloorDepth(row, column);

            if(stackedFurniture != null) {
                if(!stackedFurniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Stackable))
                    return 0;

                depth = stackedFurniture.Position.Depth + stackedFurniture.GetDimension().Depth;
            }

            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            long furniture = 0;

            using(MySqlCommand command = new MySqlCommand("INSERT INTO room_furnitures (room, furniture, row, `column`, depth, direction) VALUES (@room, @furniture, @row, @column, @depth, @direction)", connection)) {
                command.Parameters.AddWithValue("@room", client.User.Room.Id);
                command.Parameters.AddWithValue("@furniture", userFurniture.Id);
                command.Parameters.AddWithValue("@row", row);
                command.Parameters.AddWithValue("@column", column);
                command.Parameters.AddWithValue("@depth", depth);
                command.Parameters.AddWithValue("@direction", direction);

                if(command.ExecuteNonQuery() == 0)
                    return 0;

                furniture = command.LastInsertedId;
            }

            GameRoomFurniture roomFurniture = new GameRoomFurniture((int)furniture, userFurniture.Id, new GameRoomPoint(row, column, depth, direction));

            client.User.Room.Send(new SocketMessage("OnRoomEntityAdd", new { furnitures = roomFurniture }).Compose());

            client.Send(new SocketMessage("OnRoomFurniturePlace", true).Compose());

            client.User.Room.Furnitures.Add(roomFurniture);

            userFurniture.Room = client.User.Room.Id;

            using(MySqlCommand command = new MySqlCommand("UPDATE user_furnitures SET room = @room WHERE id = @id", connection)) {
                command.Parameters.AddWithValue("@id", userFurniture.Id);
                command.Parameters.AddWithValue("@room", client.User.Room.Id);

                command.ExecuteNonQuery();
            }

            return 1;
        }
    }
}
