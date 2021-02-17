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

namespace Server.Game.Rooms.Actions {
    class GameRoomFurniturePositionAction : IGameRoomFurnitureAction {
        public GameRoomFurniture Furniture { get; set; }

        public string Key => "position";
        public object Value  { get; set; }

        public GameRoomPoint Position;

        public int Speed;

        public GameRoomFurniturePositionAction(GameRoomFurniture furniture, GameRoomPoint position, int speed = 0) {
            Furniture = furniture;

            Position = position;

            Speed = speed;
        }

        public int Execute() {
            Furniture.Position = Position;

            Value = new {
                row = Position.Row,
                column = Position.Column,
                depth = Position.Depth,
                direction = Position.Direction,
                speed = Speed
            };

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

            return -1;
        }
    }
}
