using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Actions;
using Server.Game.Rooms;

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

        [JsonIgnore]
        public string Logic;

        [JsonProperty("flags")]
        public GameFurnitureFlags Flags;

        [JsonProperty("dimensions")]
        public GameRoomPoint Dimension;

        [JsonIgnore]
        public int Direction;

        [JsonIgnore]
        public string Parameters;

        public GameFurniture(MySqlDataReader reader) {
            Id = reader.GetString("id");
            Line = reader.GetString("line");
            
            Title = reader.GetString("title");
            Description = reader.GetString("description");

            Logic = reader.GetString("logic");

            Flags = (GameFurnitureFlags)reader.GetInt32("flags");
            
            Direction = reader.GetInt32("direction");

            Dimension = new GameRoomPoint(reader.GetInt32("breadth"), reader.GetInt32("height"), reader.GetDouble("depth"));

            Parameters = reader.GetString("parameters");
        }
    }
}
