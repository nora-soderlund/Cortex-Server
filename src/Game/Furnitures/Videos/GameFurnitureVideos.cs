using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Actions;
using Server.Game.Rooms;

using Server.Game.Users.Furnitures;

namespace Server.Game.Furnitures.Videos {
    class GameFurnitureVideo {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("author")]
        public string Author;

        [JsonProperty("length")]
        public int Length;

        public GameFurnitureVideo() {

        }

        public GameFurnitureVideo(MySqlDataReader reader) {
            Id = reader.GetString("id");

            Title = reader.GetString("title");
            Author = reader.GetString("author");

            Length = reader.GetInt32("length");
        }
    }
}
