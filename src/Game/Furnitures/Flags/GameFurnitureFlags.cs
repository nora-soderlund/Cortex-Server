using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Actions;

using Server.Game.Users.Furnitures;

namespace Server.Game.Furnitures {
    [Flags]
    public enum GameFurnitureFlags {
        Stackable   = 1 << 0,
        Sitable     = 1 << 1,
        Standable   = 1 << 2,
        Walkable    = 1 << 3,
        Sleepable   = 1 << 4
    };
}
