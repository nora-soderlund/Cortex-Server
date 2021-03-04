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
    class GameRoomUserPosition : IGameRoomUserAction {
        public string Entity => "users";

        public string Property => "position";

        public object Result  { get; set; }

        public int Execute() {
            if(RoomUser.Position.Row == Row && RoomUser.Position.Column == Column) {
                GameRoomFurniture roomFurniture = RoomUser.User.Room.Map.GetFloorFurniture(Row, Column);

                if(roomFurniture == null)
                    return 0;

                if(!roomFurniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Sitable))
                    return 0;

                
                if(!RoomUser.Actions.Contains("Sit"))
                    RoomUser.Actions.Add("Sit");

                Result = new {
                    row = RoomUser.Position.Row,
                    column = RoomUser.Position.Column,
                    depth = RoomUser.Position.Depth,
                    speed = 0,

                    direction = roomFurniture.Position.Direction,

                    actions = RoomUser.Actions
                };

                return -1;
            }

            Position[] path = RoomUser.User.Room.Map.GetFloorPath(RoomUser.Position, new GameRoomPoint(Row, Column));

            if(path.Length < 2)
                return 0;

            double depth = RoomUser.User.Room.Map.GetFloorDepth(path[1].X, path[1].Y);

            GameRoomFurniture furniture = RoomUser.User.Room.Map.GetFloorFurniture(path[1].X, path[1].Y);

            if(furniture != null) {
                depth = furniture.Position.Depth;

                if(!furniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Sitable))
                    depth += furniture.GetDimension().Depth;
                else
                    depth += .5;
            }

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

            Dictionary<string, object> result = new Dictionary<string, object>();

            result.Add("row", RoomUser.Position.Row);
            result.Add("column", RoomUser.Position.Column);
            result.Add("depth", RoomUser.Position.Depth);
            result.Add("direction", RoomUser.Position.Direction);
            result.Add("speed", Speed);

            if(RoomUser.Actions.Contains("Sit")) {
                RoomUser.Actions.Remove("Sit");

                result.Add("actions", RoomUser.Actions);
            }

            Result = result;

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
