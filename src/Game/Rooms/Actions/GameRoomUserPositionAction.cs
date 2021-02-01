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
    class GameRoomUserPositionAction : IGameRoomUserAction {
        public GameRoomUser User { get; set; }

        public string Key => "position";
        public object Value  { get; set; }

        public double Row;
        public double Column;

        public GameRoomUserPositionAction(GameRoomUser user, double row, double column) {
            User = user;

            Row = row;
            Column = column;
        }

        public int Execute() {
            if(User.Position.Row == Row && User.Position.Column == Column)
                return 0;

            Position[] path = User.User.Room.Map.GetPath(User.Position, new GameRoomPoint(Row, Column));

            if(path.Length < 2)
                return 0;

            if((User.Position.Row - 1 == path[1].X) && (User.Position.Column == path[1].Y))
                User.Position.Direction = 0;
            else if((User.Position.Row - 1 == path[1].X) && (User.Position.Column + 1 == path[1].Y))
                User.Position.Direction = 1;
            else if((User.Position.Row == path[1].X) && (User.Position.Column + 1 == path[1].Y))
                User.Position.Direction = 2;
            else if((User.Position.Row + 1 == path[1].X) && (User.Position.Column + 1 == path[1].Y))
                User.Position.Direction = 3;
            else if((User.Position.Row + 1 == path[1].X) && (User.Position.Column == path[1].Y))
                User.Position.Direction = 4;
            else if((User.Position.Row + 1 == path[1].X) && (User.Position.Column - 1 == path[1].Y))
                User.Position.Direction = 5;
            else if((User.Position.Row == path[1].X) && (User.Position.Column - 1 == path[1].Y))
                User.Position.Direction = 6;
            else if((User.Position.Row - 1 == path[1].X) && (User.Position.Column - 1 == path[1].Y))
                User.Position.Direction = 7;


            User.Position.Row = path[1].X;
            User.Position.Column = path[1].Y;
            User.Position.Depth = User.User.Room.Map.GetDepth((int)User.Position.Row, (int)User.Position.Column);

            Value = User.Position;

            return 1;
        }
    }
}
