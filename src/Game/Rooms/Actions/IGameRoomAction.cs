using System;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Furnitures;
using Cortex.Server.Game.Rooms.Map;

using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Rooms.Actions {
    interface IGameRoomAction {
        object Result { get; set; }

        int Execute();
    }
    
    interface IGameRoomEntityAction : IGameRoomAction {
        string Entity { get; }

        string Property { get; }
    }
    
    interface IGameRoomUserAction : IGameRoomEntityAction {
        GameRoomUser RoomUser { get; set; }
    }
    
    interface IGameRoomFurnitureAction : IGameRoomEntityAction {
        GameRoomFurniture RoomFurniture { get; set; }
    }
}
