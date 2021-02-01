using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Actions;

using Server.Game.Furnitures;
using Server.Game.Users.Furnitures;

namespace Server.Game.Rooms.Furnitures {
    class GameRoomFurniture {
        [JsonIgnore]
        public int Id;

        [JsonProperty("position")]
        public GameRoomPoint Position;

        [JsonProperty("furniture")]
        public GameFurniture Furniture;

        [JsonIgnore]
        public GameUserFurniture UserFurniture;

        public GameRoomFurniture(MySqlDataReader furniture) {
            Id = furniture.GetInt32("id");

            UserFurniture = GameFurnitureManager.GetGameUserFurniture(furniture.GetInt32("furniture"));

            Furniture = UserFurniture.Furniture;

            Position = new GameRoomPoint(furniture.GetDouble("row"), furniture.GetDouble("column"), furniture.GetDouble("depth"), furniture.GetInt32("direction"));
        }
    }
}
