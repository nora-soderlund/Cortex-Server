using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Events;

namespace Server.Game.Achievements {
    class GameAchievementLevel {
        public int Id;

        public int Level;

        public int Score;
        
        public GameAchievementLevel(MySqlDataReader reader) {
            Id = reader.GetInt32("id");
            Level = reader.GetInt32("level");
            Score = reader.GetInt32("score");
        }
    }
}
