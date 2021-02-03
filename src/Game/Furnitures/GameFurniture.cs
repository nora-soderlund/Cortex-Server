using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Actions;

using Server.Game.Users.Furnitures;

namespace Server.Game.Furnitures {
    class GameFurniture {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("line")]
        public string Line;

        public GameFurniture(MySqlDataReader reader) {
            Id = reader.GetString("id");

            Line = reader.GetString("line");
        }
    }
}
