using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms.Actions;
using Cortex.Server.Game.Rooms.Users;

using Cortex.Server.Game.Furnitures;

using Cortex.Server.Game.Users.Furnitures;

namespace Cortex.Server.Game.Rooms.Furnitures.Logics {
    interface IGameRoomFurnitureIntervalLogic : IGameRoomFurnitureLogic {
        int Interval => 1000;
        int IntervalCount { get { return 0; } set { } }

        void OnTimerPrepare() { }
        void OnTimerElapsed() { }
    }
}
