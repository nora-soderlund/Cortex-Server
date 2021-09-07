using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Events;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Messages;
using Cortex.Server.Socket.Events;

namespace Cortex.Server.Game.Rooms.Categories {
    class GameRoomCategory {
        [JsonProperty("id")]
        public long Id;
        
        [JsonProperty("name")]
        public string Name;

        public GameRoomCategory(MySqlDataReader reader) {
            Id = reader.GetInt64("id");

            Name = reader.GetString("name");
        }
    }

    class GameRoomCategories : IInitializationEvent {
        public static List<GameRoomCategory> Categories = new List<GameRoomCategory>();

        public void OnInitialization() {
            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("SELECT * FROM room_categories ORDER BY name ASC", connection))
                using(MySqlDataReader reader = command.ExecuteReader()) {
                    while(reader.Read())
                        Categories.Add(new GameRoomCategory(reader));

                    Console.WriteLine("Read " + Categories.Count + " room categories to the memory...");
                }
            }
        }

        class OnRoomCategoriesUpdate : ISocketEvent {
            public string Event => "OnRoomCategoriesUpdate";

            public int Execute(SocketClient client, JToken data) {
                client.Send(new SocketMessage("OnRoomCategoriesUpdate", Categories).Compose());
                
                return 1;
            }
        }
    }
}
