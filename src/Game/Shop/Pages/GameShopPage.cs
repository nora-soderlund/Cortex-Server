using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

using Server.Game.Users;

using Server.Game.Rooms.Map;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Furnitures;
using Server.Game.Rooms.Navigator;
using Server.Game.Rooms.Navigator.Messages;

using Server.Socket.Messages;

using Server.Game.Shop.Furnitures;

namespace Server.Game.Shop {
    class GameShopPage {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("parent")]
        public int Parent;

        [JsonProperty("icon")]
        public int Icon;

        [JsonIgnore]
        public int Order;

        [JsonIgnore]
        public string Type;

        [JsonProperty("title")]
        public string Title;

        [JsonIgnore]
        public string Description;

        [JsonIgnore]
        public string Header;

        [JsonIgnore]
        public string Teaser;

        [JsonIgnore]
        public string Content;

        [JsonIgnore]
        public List<GameShopFurniture> Furnitures = new List<GameShopFurniture>();

        public GameShopPage(MySqlDataReader reader) {
            Id = reader.GetInt32("id");
            Parent = reader.GetInt32("parent");

            Icon = reader.GetInt32("icon");
            Order = reader.GetInt32("order");

            Type = reader.GetString("type");

            Title = reader.GetString("title");

            Description = (Convert.IsDBNull(reader["description"]))?(null):(reader.GetString("description"));
            Header = (Convert.IsDBNull(reader["header"]))?(null):(reader.GetString("header"));
            Teaser = (Convert.IsDBNull(reader["teaser"]))?(null):(reader.GetString("teaser"));
            Content = (Convert.IsDBNull(reader["content"]))?(null):(reader.GetString("content"));

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("SELECT * FROM shop_items WHERE page = @page", connection)) {
                    command.Parameters.AddWithValue("@page", Id);

                    using(MySqlDataReader data = command.ExecuteReader()) {
                        while(data.Read())
                            Furnitures.Add(new GameShopFurniture(data));
                    }
                }
            }
        }
    }
}
