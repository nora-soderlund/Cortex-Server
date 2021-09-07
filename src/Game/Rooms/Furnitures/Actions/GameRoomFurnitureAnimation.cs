using System;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Furnitures;
using Cortex.Server.Game.Rooms.Map;
using Cortex.Server.Game.Rooms.Actions;

using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Rooms.Furnitures.Actions {
    class GameRoomFurnitureAnimation : IGameRoomFurnitureAction {
        public string Entity => "furnitures";
        public string Property => "animation";

        public GameRoomFurniture RoomFurniture { get; set; }
        public object Result  { get; set; }

        public int Execute() {
            return -1;
        }

        public GameRoomFurnitureAnimation(GameRoomFurniture furniture, int animation) {
            RoomFurniture = furniture;

            Result = animation;
        }
    }
}
