using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

using Cortex.Server.Game.Users;

using Cortex.Server.Game.Rooms.Map;
using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Furnitures;
using Cortex.Server.Game.Rooms.Navigator;

using Cortex.Server.Socket.Messages;

using Cortex.Server.Events;

using Cortex.Server.Game.Shop.Furnitures;

namespace Cortex.Server.Game.Shop {
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

            Console.WriteLine("Loaded " + Pages.Count + " shop pages to memory...");
        }
    }
}
