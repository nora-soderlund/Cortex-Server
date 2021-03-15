using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Socket.Clients;

using Server.Game.Rooms;
using Server.Game.Furnitures;
using Server.Game.Users.Friends;
using Server.Game.Users.Furnitures;

namespace Server.Game.Users.Badges {
    class GameUserBadge {
        [JsonIgnore]
        public int Id;

        [JsonProperty("badge")]
        public string Badge;

        [JsonIgnore]
        public bool Equipped;

        [JsonProperty("timestamp")]
        public int Timestamp;

        public GameUserBadge(MySqlDataReader reader) {
            Id = reader.GetInt32("id");
            
            Badge = reader.GetString("badge");

            Timestamp = reader.GetInt32("timestamp");

            Equipped = reader.GetBoolean("equipped");
        }
    }
}
