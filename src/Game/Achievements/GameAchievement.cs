using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Events;

namespace Server.Game.Achievements {
    class GameAchievement {
        public string Id;

        public List<GameAchievementLevel> Levels = new List<GameAchievementLevel>();
        public int LevelCount;
        
        public GameAchievement(MySqlDataReader reader) {
            Id = reader.GetString("id");
            LevelCount = reader.GetInt32("levels");

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using MySqlCommand command = new MySqlCommand("SELECT * FROM achievement_levels WHERE achievement = @achievement", connection);
                command.Parameters.AddWithValue("@achievement", Id);

                MySqlDataReader level = command.ExecuteReader();

                while(level.Read())
                    Levels.Add(new GameAchievementLevel(level));
            }
        }
    }
}
