using System;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using Server.Game.Rooms.Users;
using Server.Game.Rooms.Furnitures;
using Server.Game.Rooms.Map;
using Server.Game.Rooms.Actions;

using Server.Socket.Messages;

namespace Server.Game.Rooms.Actions {
    class GameRoomUserPositionAction : IGameRoomUserAction {
        public GameRoomUser User { get; set; }

        public string Key => "position";
        public object Value  { get; set; }

        public int Row;
        public int Column;

        public int Speed;

        public GameRoomUserPositionAction(GameRoomUser user, int row, int column, int speed = 500) {
            User = user;

            Row = row;
            Column = column;

            Speed = speed;
        }

        public int Execute() {
            if(User.Position.Row == Row && User.Position.Column == Column)
                return 0;

            Position[] path = User.User.Room.Map.GetFloorPath(User.Position, new GameRoomPoint(Row, Column));

            if(path.Length < 2)
                return 0;

            double depth = User.User.Room.Map.GetFloorDepth(path[1].X, path[1].Y);

            GameRoomFurniture furniture = User.User.Room.Map.GetFloorFurniture(path[1].X, path[1].Y);

            if(furniture != null)
                depth = furniture.Position.Depth + furniture.GetDimension().Depth;

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
            User.Position.Depth = depth;

            Value = new {
                row = User.Position.Row,
                column = User.Position.Column,
                depth = User.Position.Depth,
                direction = User.Position.Direction,
                speed = Speed
            };

            return 1;
        }
    }
}
