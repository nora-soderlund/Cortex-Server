using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms.Actions;

using Cortex.Server.Game.Users.Furnitures;

namespace Cortex.Server.Game.Furnitures {
    [Flags]
    public enum GameFurnitureFlags {
        Stackable   = 1 << 0,
        Sitable     = 1 << 1,
        Standable   = 1 << 2,
        Walkable    = 1 << 3,
        Sleepable   = 1 << 4
    };
}
