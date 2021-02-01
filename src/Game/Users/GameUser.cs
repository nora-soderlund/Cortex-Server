using System;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Rooms;
using Server.Socket.Clients;

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

        public GameUser(MySqlDataReader reader) {
            Id = reader.GetInt32("id");

            Figure = reader.GetString("figure");

            if((Home = reader.GetInt32("home")) == 0)
                Home = null;
        }
    }
}
