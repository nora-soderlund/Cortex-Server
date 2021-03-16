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
    public enum GameUserFriendStatus {
        Sent        = -1,
        Received    = 0,

        Regular     = 1
    };
}
