using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Cortex.Server.Socket.Clients;

using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Furnitures;
using Cortex.Server.Game.Users.Friends;
using Cortex.Server.Game.Users.Furnitures;

namespace Cortex.Server.Game.Users.Achievements {
    class GameUserAchievement {
        public int Id;

        public string Achievement;

        public int Score;
        public int Level;

        public GameUserAchievement() {
            
        }

        public GameUserAchievement(MySqlDataReader reader) {
            Id = reader.GetInt32("id");

            Achievement = reader.GetString("achievement");

            Score = reader.GetInt32("score");
            Level = reader.GetInt32("level");
        }
    }
}
