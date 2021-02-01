using System;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using Server.Game.Rooms.Users;
using Server.Game.Rooms.Map;
using Server.Game.Rooms.Actions;

using Server.Socket.Messages;

namespace Server.Game.Rooms.Actions {
    enum GameRoomUserActionType { Add, Remove };

    class GameRoomUserAction : IGameRoomUserAction {
        public GameRoomUser User { get; set; }

        public string Key => "action";
        public object Value  { get; set; }

        public string Action;

        public GameRoomUserActionType Type;

        public GameRoomUserAction(GameRoomUser user, string action, GameRoomUserActionType type) {
            User = user;

            Action = action;

            Type = type;
        }

        public int Execute() {
            switch(Type) {
                case GameRoomUserActionType.Add: {
                    if(User.Actions.Contains(Action))
                        return 0;

                    User.Actions.Add(Action);

                    Value = Action;

                    break;
                }

                case GameRoomUserActionType.Remove: {
                    if(!User.Actions.Contains(Action))
                        return 0;

                    User.Actions.Remove(Action);

                    Value = User.Actions;

                    break;
                }
            }

            return -1;
        }
    }
}
