using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Socket.Clients;

using Server.Game.Rooms;
using Server.Game.Furnitures;
using Server.Game.Users.Friends;
using Server.Game.Users.Furnitures;

namespace Server.Game.Users.Achievements {
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
