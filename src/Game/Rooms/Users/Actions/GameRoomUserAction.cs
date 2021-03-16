using System;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using MySql.Data.MySqlClient;

using Server.Game.Furnitures;

using Server.Game.Rooms.Users;
using Server.Game.Rooms.Map;
using Server.Game.Rooms.Actions;
using Server.Game.Rooms.Furnitures;

using Server.Socket.Messages;

namespace Server.Game.Rooms.Users.Actions {
    class GameRoomUserAction : IGameRoomUserAction {
        public string Entity => "users";

        public string Property => "action";

        public object Result  { get; set; }

        public int Execute() {
            Result = new {
                action = Action,
                time = Time
            };

            return -1;
        }

        public GameRoomUser RoomUser { get; set; }

        public string Action;
        public int Time;

        public GameRoomUserAction(GameRoomUser roomUser, string action, int time) {
            RoomUser = roomUser;

            Action = action;
            Time = time;
        }
    }
}
