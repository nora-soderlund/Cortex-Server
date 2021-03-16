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
        
        public GameAchievement(MySqlDataReader reader) {
            Id = reader.GetString("id");
        }
    }
}
