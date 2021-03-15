using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

namespace Server.Game.Badges {
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

            if(!reader.Read())
                return null;

            badge = new GameBadge(reader);

            Badges.Add(badge);

            return badge;
        }
    }
}
