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

            if(!client.User.Room.Map.IsValidColumn(row, column))
                return 0;

            GameRoomFurniture stackedFurniture = client.User.Room.Map.GetFurniture(row, column);

            if(stackedFurniture != null && !stackedFurniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Stackable))
                return 0;

            double? depth = client.User.Room.Map.GetDepth(row, column);

            roomFurniture.Position = new GameRoomPoint(row, column, depth, direction);

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE room_furnitures SET row = @row, `column` = @column, depth = @depth, direction = @direction WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", roomFurniture.Id);
                    command.Parameters.AddWithValue("@row", roomFurniture.Position.Row);
                    command.Parameters.AddWithValue("@column", roomFurniture.Position.Column);
                    command.Parameters.AddWithValue("@depth", roomFurniture.Position.Depth);
                    command.Parameters.AddWithValue("@direction", roomFurniture.Position.Direction);

                    command.ExecuteNonQuery();
                }
            }

            return 1;
        }
    }
}
