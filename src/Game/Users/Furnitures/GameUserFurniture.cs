using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms.Actions;

using Cortex.Server.Game.Furnitures;

namespace Cortex.Server.Game.Users.Furnitures {
    class GameUserFurniture {
        [JsonIgnore]
        public int Id;

        [JsonIgnore]
        public int User;

        [JsonIgnore]
        public int Room;

        [JsonIgnore]
        public GameFurniture Furniture;

        public GameUserFurniture(MySqlDataReader furniture) {
            Id = furniture.GetInt32("id");

            User = furniture.GetInt32("user");
            Room = furniture.GetInt32("room");

            Furniture = GameFurnitureManager.GetGameFurniture(furniture.GetString("furniture"));
        }
    }
}
