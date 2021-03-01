using System;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using MySql.Data.MySqlClient;

using Server.Game.Rooms.Users;
using Server.Game.Rooms.Furnitures;
using Server.Game.Rooms.Map;
using Server.Game.Rooms.Actions;

using Server.Socket.Messages;

namespace Server.Game.Rooms.Furnitures.Actions {
    class GameRoomFurniturePosition : IGameRoomAction {
        public object Result  { get; set; }

        public int Execute() {
            Result = new {
                id = Furniture.Id,
                
                position = new {
                    row = Furniture.Position.Row,
                    column = Furniture.Position.Column,
                    depth = Furniture.Position.Depth,
                    direction = Furniture.Position.Direction,
                    speed = Speed
                }
            };

            return -1;
        }

        public GameRoomFurniturePosition(GameRoomFurniture furniture, GameRoomPoint position, int speed = 0) {
            Furniture = furniture;

            Furniture.Position = position;

            Speed = speed;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE room_furnitures SET row = @row, `column` = @column, depth = @depth, direction = @direction WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", Furniture.Id);
                    command.Parameters.AddWithValue("@row", Furniture.Position.Row);
                    command.Parameters.AddWithValue("@column", Furniture.Position.Column);
                    command.Parameters.AddWithValue("@depth", Furniture.Position.Depth);
                    command.Parameters.AddWithValue("@direction", Furniture.Position.Direction);

                    command.ExecuteNonQuery();
                }
            }
        }

        public GameRoomFurniture Furniture { get; set; }

        public int Speed;
    }
}
