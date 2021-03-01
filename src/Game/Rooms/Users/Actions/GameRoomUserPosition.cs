using System;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using MySql.Data.MySqlClient;

using Server.Game.Rooms.Users;
using Server.Game.Rooms.Map;
using Server.Game.Rooms.Actions;
using Server.Game.Rooms.Furnitures;

using Server.Socket.Messages;

namespace Server.Game.Rooms.Users.Actions {
    class GameRoomUserPosition : IGameRoomUserAction {
        public string Entity => "users";

        public string Property => "position";

        public object Result  { get; set; }

        public int Execute() {
            if(RoomUser.Position.Row == Row && RoomUser.Position.Column == Column)
                return 0;

            Position[] path = RoomUser.User.Room.Map.GetFloorPath(RoomUser.Position, new GameRoomPoint(Row, Column));

            if(path.Length < 2)
                return 0;

            double depth = RoomUser.User.Room.Map.GetFloorDepth(path[1].X, path[1].Y);

            GameRoomFurniture furniture = RoomUser.User.Room.Map.GetFloorFurniture(path[1].X, path[1].Y);

            if(furniture != null)
                depth = furniture.Position.Depth + furniture.GetDimension().Depth;

            if((RoomUser.Position.Row - 1 == path[1].X) && (RoomUser.Position.Column == path[1].Y))
                RoomUser.Position.Direction = 0;
            else if((RoomUser.Position.Row - 1 == path[1].X) && (RoomUser.Position.Column + 1 == path[1].Y))
                RoomUser.Position.Direction = 1;
            else if((RoomUser.Position.Row == path[1].X) && (RoomUser.Position.Column + 1 == path[1].Y))
                RoomUser.Position.Direction = 2;
            else if((RoomUser.Position.Row + 1 == path[1].X) && (RoomUser.Position.Column + 1 == path[1].Y))
                RoomUser.Position.Direction = 3;
            else if((RoomUser.Position.Row + 1 == path[1].X) && (RoomUser.Position.Column == path[1].Y))
                RoomUser.Position.Direction = 4;
            else if((RoomUser.Position.Row + 1 == path[1].X) && (RoomUser.Position.Column - 1 == path[1].Y))
                RoomUser.Position.Direction = 5;
            else if((RoomUser.Position.Row == path[1].X) && (RoomUser.Position.Column - 1 == path[1].Y))
                RoomUser.Position.Direction = 6;
            else if((RoomUser.Position.Row - 1 == path[1].X) && (RoomUser.Position.Column - 1 == path[1].Y))
                RoomUser.Position.Direction = 7;

            RoomUser.Position.Row = path[1].X;
            RoomUser.Position.Column = path[1].Y;
            RoomUser.Position.Depth = depth;

            Result = new {
                row = RoomUser.Position.Row,
                column = RoomUser.Position.Column,
                depth = RoomUser.Position.Depth,
                direction = RoomUser.Position.Direction,
                speed = Speed
            };

            return 1;
        }

        public GameRoomUser RoomUser { get; set; }

        public int Speed;

        public int Row;
        public int Column;

        public GameRoomUserPosition(GameRoomUser roomUser, int row, int column, int speed = 500) {
            RoomUser = roomUser;

            Speed = speed;

            Row = row;

            Column = column;
        }
    }
}
