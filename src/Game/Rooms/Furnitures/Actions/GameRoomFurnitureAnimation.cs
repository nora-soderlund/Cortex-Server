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
    class GameRoomFurnitureAnimation : IGameRoomFurnitureAction {
        public string Entity => "furnitures";
        public string Property => "animation";

        public GameRoomFurniture RoomFurniture { get; set; }
        public object Result  { get; set; }

        public int Execute() {
            Result = RoomFurniture.Animation;

            return -1;
        }

        public GameRoomFurnitureAnimation(GameRoomFurniture furniture, int animation, bool change = true) {
            RoomFurniture = furniture;

            if(change)
                RoomFurniture.Animation = animation;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE room_furnitures SET animation = @animation WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", RoomFurniture.Id);
                    command.Parameters.AddWithValue("@animation", RoomFurniture.Animation);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
