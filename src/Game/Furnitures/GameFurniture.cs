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

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("flags")]
        public GameFurnitureFlags Flags;

        [JsonIgnore]
        public int Direction;

        public GameFurniture(MySqlDataReader reader) {
            Id = reader.GetString("id");
            Line = reader.GetString("line");
            
            Title = reader.GetString("title");
            Description = reader.GetString("description");

            Flags = (GameFurnitureFlags)reader.GetInt32("flags");
            
            Direction = reader.GetInt32("direction");
        }
    }
}
