using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Actions;

using Server.Game.Users.Furnitures;

namespace Server.Game.Furnitures {
    class GameFurniture {
        [JsonIgnore]
        public int Id;

        [JsonProperty("line")]
        public string Line;

        [JsonProperty("asset")]
        public string Asset;

        public GameFurniture(MySqlDataReader reader) {
            Id = reader.GetInt32("id");

            Line = reader.GetString("line");
            Asset = reader.GetString("asset");
        }
    }
}
