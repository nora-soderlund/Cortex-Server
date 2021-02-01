using System;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using Server.Game.Rooms.Users;
using Server.Game.Rooms.Map;

using Server.Socket.Messages;

namespace Server.Game.Rooms.Actions {
    interface IGameRoomUserAction {
        GameRoomUser User { get; set; }

        string Key { get; }

        object Value { get; set; }

        int Execute();
    }
}
