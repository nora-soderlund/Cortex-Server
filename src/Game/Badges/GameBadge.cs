using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

namespace Server.Game.Badges {
    class GameBadge {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("description")]
        public string Description;

        public GameBadge() {

        }

        public GameBadge(MySqlDataReader reader) {
            Id = reader.GetString("id");

            Title = reader.GetString("title");
            Description = reader.GetString("description");
        }
    }
}
