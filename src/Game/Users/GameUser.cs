using System;
using System.Linq;
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

        public Dictionary<string, Dictionary<string, int>> GetFurnitureMessages(string id = "") {
            Dictionary<string, Dictionary<string, int>> furnitures = new Dictionary<string, Dictionary<string, int>>();

            foreach(GameUserFurniture furniture in (id.Length == 0)?(Furnitures):(Furnitures.Where(x => x.Furniture.Id == id))) {
                if(!furnitures.ContainsKey(furniture.Furniture.Id))
                    furnitures.Add(furniture.Furniture.Id, new Dictionary<string, int>());

                if(furniture.Room != 0) {
                    if(!furnitures[furniture.Furniture.Id].ContainsKey("room"))
                        furnitures[furniture.Furniture.Id].Add("room", 0);

                    furnitures[furniture.Furniture.Id]["room"]++;
                }
                else {
                    if(!furnitures[furniture.Furniture.Id].ContainsKey("inventory"))
                        furnitures[furniture.Furniture.Id].Add("inventory", 0);

                    furnitures[furniture.Furniture.Id]["inventory"]++;
                }
            }

            return furnitures;
        }
    }
}
