using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Cortex.Server.Socket.Clients;

using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Furnitures;
using Cortex.Server.Game.Users.Friends;
using Cortex.Server.Game.Users.Furnitures;

namespace Cortex.Server.Game.Users.Badges {
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
