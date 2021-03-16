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

        [JsonProperty("equipped")]
        public bool Equipped;

        [JsonIgnore]
        public long EquippedTimestamp;

        [JsonProperty("timestamp")]
        public long Timestamp;

        public GameUserBadge() {
            
        }

        public GameUserBadge(MySqlDataReader reader) {
            Id = reader.GetInt32("id");
            
            Badge = reader.GetString("badge");

            Timestamp = reader.GetUInt32("timestamp");

            Equipped = reader.GetBoolean("equipped");
            EquippedTimestamp = reader.GetUInt32("equipped_timestamp");
        }
    }
}
