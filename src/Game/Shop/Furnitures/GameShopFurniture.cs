using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

using Cortex.Server.Game.Users;

using Cortex.Server.Game.Furnitures;

using Cortex.Server.Game.Rooms.Map;
using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Furnitures;
using Cortex.Server.Game.Rooms.Navigator;

using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Shop.Furnitures {
    class GameShopFurniture {
        [JsonProperty("id")]
        public int Id;
        
        [JsonIgnore]
        public int Page;
        
        
        [JsonProperty("furniture")]
        public GameFurniture Furniture;

        
        [JsonProperty("credits")]
        public int? Credits;
        
        [JsonProperty("duckets")]
        public int? Duckets;
        
        [JsonProperty("diamonds")]
        public int? Diamonds;

        public GameShopFurniture(MySqlDataReader reader) {
            Id = reader.GetInt32("id");
            Page = reader.GetInt32("shop");
            
            Furniture = GameFurnitureManager.GetGameFurniture(reader.GetString("furniture"));
        }
    }
}
