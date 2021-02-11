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

using Server.Events;

using Server.Game.Shop.Furnitures;

namespace Server.Game.Shop {
    class GameShop : IInitializationEvent {
        public static List<GameShopPage> Pages = new List<GameShopPage>();
        public static List<GameShopFurniture> Furnitures = new List<GameShopFurniture>();

        public void OnInitialization() {
            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            using(MySqlCommand command = new MySqlCommand("SELECT * FROM shop", connection)) 
            using(MySqlDataReader reader = command.ExecuteReader()) {
                while(reader.Read())
                    Pages.Add(new GameShopPage(reader));
            }

            Pages.OrderBy(x => x.Parent).ThenBy(x => x.Order).ToList();
        }
    }
}
