using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Badges;

using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Badges {
    class GameBadgeManager {
        public static List<GameBadge> Badges = new List<GameBadge>();

        public static GameBadge GetBadge(string id) {
            GameBadge badge = Badges.Find(x => x.Id == id);

            if(badge != null)
                return badge;

            using MySqlConnection connection = new MySqlConnection(Program.Connection);
            connection.Open();

            using MySqlCommand command = new MySqlCommand("SELECT * FROM badges WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if(!reader.Read()) {
                badge = new GameBadge() {
                    Id = id,
                    Title = id,
                    Description = "No badge description..."
                };

                Badges.Add(badge);

                return badge;
            }

            badge = new GameBadge(reader);

            Badges.Add(badge);

            return badge;
        }

        public static GameUserBadge AddBadge(GameUser user, string badge, bool notify) {
            if(user.Badges.Find(x => x.Badge == badge) != null)
                return null;

            using MySqlConnection connection = new MySqlConnection(Program.Connection);
            connection.Open();

            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            using MySqlCommand command = new MySqlCommand("INSERT INTO user_badges (user, badge, timestamp) VALUES (@user, @badge, @timestamp)", connection);
            command.Parameters.AddWithValue("@user", user.Id);
            command.Parameters.AddWithValue("@badge", badge);
            command.Parameters.AddWithValue("@timestamp", timestamp);

            command.ExecuteNonQuery();

            GameUserBadge userBadge = new GameUserBadge() {
                Id = (int)command.LastInsertedId,
                Badge = badge,
                Equipped = false,
                Timestamp = timestamp
            };

            user.Badges.Add(userBadge);

            if(notify)
                user.Client.Send(new SocketMessage("OnUserBadgeAdd", userBadge).Compose());

            return userBadge;
        }
    }
}
