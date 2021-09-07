using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Rooms;
using Cortex.Server.Socket.Clients;

using Cortex.Server.Game.Furnitures;
using Cortex.Server.Game.Users.Furnitures;

namespace Cortex.Server.Game.Users.Friends {
    public enum GameUserFriendStatus {
        Sent        = -1,
        Received    = 0,

        Regular     = 1
    };
}
