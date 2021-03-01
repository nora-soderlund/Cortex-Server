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
    class GameRoomFurnitureAnimation : IGameRoomAction {
        public object Result  { get; set; }

        public int Execute() {
            Result = new { id = Furniture.Id, animation = Furniture.Animation };

            return -1;
        }

        public GameRoomFurniture Furniture { get; set; }

        public GameRoomFurnitureAnimation(GameRoomFurniture furniture, int animation) {
            Furniture = furniture;

            Furniture.Animation = animation;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE room_furnitures SET animation = @animation WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", Furniture.Id);
                    command.Parameters.AddWithValue("@animation", Furniture.Animation);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
