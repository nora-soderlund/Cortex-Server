using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

using Server.Game.Users;

using Server.Game.Rooms.Map;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Actions;
using Server.Game.Rooms.Furnitures;
using Server.Game.Rooms.Navigator;

using Server.Socket.Messages;
using Server.Socket;

namespace Server.Game {
    class Game {
        public static GameUser GetUser(long id) {
            return Program.Socket.clients.Find(x => x.User != null && x.User.Id == id).User;
        }
    }
}
