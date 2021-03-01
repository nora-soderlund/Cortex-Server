using System;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using Server.Game.Rooms.Users;
using Server.Game.Rooms.Map;

using Server.Socket.Messages;

namespace Server.Game.Rooms.Actions {
    interface IGameRoomAction {
        object Result { get; set; }

        int Execute();
    }
}
