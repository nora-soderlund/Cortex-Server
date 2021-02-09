using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Events;

using Server.Game.Rooms.Navigator;

using Server.Socket.Clients;
using Server.Socket.Messages;
using Server.Socket.Events;

namespace Server.Game.Rooms.Navigator.Messages {
    class GameRoomNavigatorMessage {
        [JsonProperty("id")]
        public int Id;

        [JsonIgnore]
        public int Owner;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("users")]
        public int? Users = null;

        [JsonIgnore]
        public int Access;

        [JsonIgnore]
        public string Password;

        public GameRoomNavigatorMessage(MySqlDataReader reader) {
            Id = reader.GetInt32("id");
            Owner = reader.GetInt32("owner");
            Title = reader.GetString("title");

            Access = reader.GetInt32("access");

            if(reader["password"] != DBNull.Value)
                Password = reader.GetString("password");
        }

        public void UpdateUsers(int users) {
            if((Users = users) == 0)
                Users = null;

            GameRoomNavigator.UpdateRooms();
        }
    }
}
