using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

using Server.Game.Users;

using Server.Game.Furnitures;

using Server.Game.Rooms.Map;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Furnitures;
using Server.Game.Rooms.Navigator;
using Server.Game.Rooms.Navigator.Messages;

using Server.Socket.Messages;

namespace Server.Game.Shop.Furnitures {
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
