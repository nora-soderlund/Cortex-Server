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
    class GameRoomFurnitureAnimationAction : IGameRoomFurnitureAction {
        public GameRoomFurniture Furniture { get; set; }

        public string Key => "animation";
        public object Value  { get; set; }

        public int Animation;

        public GameRoomFurnitureAnimationAction(GameRoomFurniture furniture, int animation) {
            Furniture = furniture;
            
            Animation = animation;
        }

        public int Execute() {
            Furniture.Animation = Animation;

            Value = Animation;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE room_furnitures SET animation = @animation WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", Furniture.Id);
                    command.Parameters.AddWithValue("@animation", Furniture.Animation);

                    command.ExecuteNonQuery();
                }
            }

            return -1;
        }
    }
}
