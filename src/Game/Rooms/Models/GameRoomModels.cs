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

namespace Cortex.Server.Game.Rooms.Models {
    class GameRoomModel {
        [JsonProperty("id")]
        public long Id;
        
        [JsonProperty("map")]
        public string Map;
        
        [JsonProperty("door")]
        public GameRoomPoint Door;

        public GameRoomModel(MySqlDataReader reader) {
            Id = reader.GetInt64("id");

            Map = reader.GetString("map").ToUpper();

            Door = new GameRoomPoint(reader.GetInt32("door_row"), reader.GetInt32("door_column"), 0, reader.GetInt32("door_direction"));
        }
    }

    class GameRoomModels : IInitializationEvent {
        public static List<GameRoomModel> Models = new List<GameRoomModel>();

        public void OnInitialization() {
            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("SELECT * FROM room_models WHERE enabled = 1 ORDER BY LENGTH(map) ASC", connection))
                using(MySqlDataReader reader = command.ExecuteReader()) {
                    while(reader.Read())
                        Models.Add(new GameRoomModel(reader));

                    Console.WriteLine("Read " + Models.Count + " room models to the memory...");
                }
            }
        }

        class OnRoomModelsUpdate : ISocketEvent {
            public string Event => "OnRoomModelsUpdate";

            public int Execute(SocketClient client, JToken data) {
                client.Send(new SocketMessage("OnRoomModelsUpdate", Models).Compose());
                
                return 1;
            }
        }
    }
}
