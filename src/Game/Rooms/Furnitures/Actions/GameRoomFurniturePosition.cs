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
    class GameRoomFurniturePosition : IGameRoomFurnitureAction {
        public string Entity => "furnitures";
        public string Property => "position";

        public GameRoomFurniture RoomFurniture { get; set; }

        public object Result { get; set; }

        public int Execute() {
            Result = new {
                row = RoomFurniture.Position.Row,
                column = RoomFurniture.Position.Column,
                depth = RoomFurniture.Position.Depth,
                direction = RoomFurniture.Position.Direction,
                speed = Speed
            };

            foreach(GameRoomFurniture stacked in RoomFurniture.Room.Furnitures.FindAll(x => x.Position.Row == RoomFurniture.Position.Row && x.Position.Column == RoomFurniture.Position.Column)) {
                if(stacked.Id == RoomFurniture.Id)
                    continue;

                if(stacked.Logic == null)
                    continue;

                stacked.Logic.OnFurnitureEnter(RoomFurniture);
            }

            return -1;
        }

        public GameRoomFurniturePosition(GameRoomFurniture furniture, GameRoomPoint position, int speed = 0) {
            RoomFurniture = furniture;

            foreach(GameRoomFurniture stacked in RoomFurniture.Room.Furnitures.FindAll(x => x.Position.Row == RoomFurniture.Position.Row && x.Position.Column == RoomFurniture.Position.Column)) {
                if(stacked.Id == RoomFurniture.Id)
                    continue;

                if(stacked.Logic == null)
                    continue;

                stacked.Logic.OnFurnitureLeave(RoomFurniture);
            }

            RoomFurniture.Position = position;

            Speed = speed;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE room_furnitures SET row = @row, `column` = @column, depth = @depth, direction = @direction WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", RoomFurniture.Id);
                    command.Parameters.AddWithValue("@row", RoomFurniture.Position.Row);
                    command.Parameters.AddWithValue("@column", RoomFurniture.Position.Column);
                    command.Parameters.AddWithValue("@depth", RoomFurniture.Position.Depth);
                    command.Parameters.AddWithValue("@direction", RoomFurniture.Position.Direction);

                    command.ExecuteNonQuery();
                }
            }
        }

        public int Speed;
    }
}
