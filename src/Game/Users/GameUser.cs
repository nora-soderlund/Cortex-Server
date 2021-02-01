using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Rooms;
using Server.Socket.Clients;

using Server.Game.Furnitures;
using Server.Game.Users.Furnitures;

namespace Server.Game.Users {
    class GameUser {
        [JsonIgnore]
        public int Id;

        [JsonIgnore]
        public SocketClient Client;

        [JsonProperty("home")]
        public int? Home;

        [JsonIgnore]
        public string Figure;

        [JsonIgnore]
        public GameRoom Room = null;

        [JsonIgnore]
        public List<GameUserFurniture> Furnitures = new List<GameUserFurniture>();

        public GameUser(MySqlDataReader reader) {
            Id = reader.GetInt32("id");

            Figure = reader.GetString("figure");

            if((Home = reader.GetInt32("home")) == 0)
                Home = null;

            GetFurnitures();
        }

        public void GetFurnitures() {
            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            using MySqlCommand command = new MySqlCommand("SELECT * FROM user_furnitures WHERE user = @user", connection);
            command.Parameters.AddWithValue("@user", Id);

            MySqlDataReader reader = command.ExecuteReader();

            while(reader.Read()) {
                Furnitures.Add(GameFurnitureManager.GetGameUserFurniture(reader.GetInt32("id")));
            }
        }
    }
}
