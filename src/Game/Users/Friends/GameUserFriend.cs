using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Rooms;
using Server.Socket.Clients;

using Server.Game.Furnitures;
using Server.Game.Users.Furnitures;

namespace Server.Game.Users.Friends {
    class GameUserFriend {
        [JsonIgnore]
        public int User;

        [JsonProperty("friend")]
        public int Friend;

        [JsonProperty("status")]
        public GameUserFriendStatus Status;

        public GameUserFriend() {
            
        }

        public GameUserFriend(MySqlDataReader reader) {
            User = reader.GetInt32("user");
            Friend = reader.GetInt32("friend");

            Status = (GameUserFriendStatus)reader.GetInt32("status");
        }

        public void SetStatus(GameUserFriendStatus status) {
            Status = status;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                MySqlCommand command = new MySqlCommand("UPDATE user_friends SET status = @status WHERE user = @user AND friend = @friend", connection);

                command.Parameters.AddWithValue("@status", Status);
                command.Parameters.AddWithValue("@user", User);
                command.Parameters.AddWithValue("@friend", Friend);

                command.ExecuteNonQuery();
            }
        }
    }
}
