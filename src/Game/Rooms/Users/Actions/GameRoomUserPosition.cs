using System;
using System.Linq;
using System.Collections.Generic;
using System.Timers;

using Newtonsoft.Json;

using RoyT.AStar;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Furnitures;

using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Map;
using Cortex.Server.Game.Rooms.Actions;
using Cortex.Server.Game.Rooms.Furnitures;

using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Rooms.Users.Actions {
    class GameRoomUserPosition : IGameRoomUserAction {
        public string Entity => "users";

        public string Property => "position";

        public object Result  { get; set; }

        public int Execute() {
            if(RoomUser.Position.Row == Row && RoomUser.Position.Column == Column) {
                if(Depth != -1) {
                    Dictionary<string, object> results = new Dictionary<string, object>();

                    results.Add("row", RoomUser.Position.Row);
                    results.Add("column", RoomUser.Position.Column);
                    results.Add("depth", Depth);
                    results.Add("direction", RoomUser.Position.Direction);
                    results.Add("speed", Speed);

                    Result = results;

                    return -1;
                }

                GameRoomFurniture roomFurniture = RoomUser.User.Room.Map.GetFloorFurniture(Row, Column);

                if(roomFurniture == null)
                    return 0;

                if(!roomFurniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Sitable)) {
                    roomFurniture = RoomUser.User.Room.Map.GetFloorFurniture(Row, Column, null, GameFurnitureFlags.Sitable);

                    if(roomFurniture == null)
                        return 0;
                }

                
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

                RoomUser.Position.Direction = roomFurniture.Position.Direction;

                return -1;
            }
            
            Position[] path = RoomUser.User.Room.Map.GetFloorPath(RoomUser.Position, new GameRoomPoint(Row, Column));

            if(path.Length < 2)
                return 0;

            double depth = RoomUser.User.Room.Map.GetFloorDepth(path[1].X, path[1].Y);

            GameRoomFurniture furniture = RoomUser.User.Room.Map.GetFloorFurniture(path[1].X, path[1].Y);

            if(furniture != null) {
                depth = furniture.Position.Depth + furniture.GetDimension().Depth;

                if(furniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Sitable))
                    depth -= .5;
                else {
                    if(!furniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Sitable)) {
                        furniture = RoomUser.User.Room.Map.GetFloorFurniture(path[1].X, path[1].Y, null, GameFurnitureFlags.Sitable);

                        if(furniture != null)
                            depth = furniture.Position.Depth + furniture.GetDimension().Depth - .5;
                    }
                }
            }

            foreach(GameRoomFurniture logicFurniture in RoomUser.User.Room.Furnitures.Where(x => x.Position.Row == RoomUser.Position.Row && x.Position.Column == RoomUser.Position.Column))
                logicFurniture.Logic.OnUserLeave(RoomUser);
            
            GameRoomFurniture logicLeaveFurniture = RoomUser.User.Room.Map.GetFloorFurniture(RoomUser.Position.Row, RoomUser.Position.Column);

            
            if(Walk)
                RoomUser.Position.Direction = GameRoomDirection.FromPosition(RoomUser.Position, new GameRoomPoint(path[1].X, path[1].Y));

            RoomUser.Position.Row = path[1].X;
            RoomUser.Position.Column = path[1].Y;
            RoomUser.Position.Depth = depth;
            
            foreach(GameRoomFurniture logicFurniture in RoomUser.User.Room.Furnitures.Where(x => x.Position.Row == RoomUser.Position.Row && x.Position.Column == RoomUser.Position.Column))
                logicFurniture.Logic.OnUserEnter(RoomUser);

            Dictionary<string, object> result = new Dictionary<string, object>();

            result.Add("row", RoomUser.Position.Row);
            result.Add("column", RoomUser.Position.Column);
            result.Add("depth", RoomUser.Position.Depth);
            result.Add("direction", RoomUser.Position.Direction);
            result.Add("speed", Speed);

            if(Walk)
                result.Add("walk", Walk);

            if(RoomUser.Actions.Contains("Sit")) {
                RoomUser.Actions.Remove("Sit");

                result.Add("actions", RoomUser.Actions);
            }

            Result = result;

            return 1;
        }

        public GameRoomUser RoomUser { get; set; }

        public int Speed;
        public bool Walk;

        public int Row;
        public int Column;
        public double Depth = -1;

        public GameRoomUserPosition(GameRoomUser roomUser, int row, int column, int speed) {
            RoomUser = roomUser;

            Speed = speed;

            Row = row;
            Column = column;

            Walk = true;
        }

        public GameRoomUserPosition(GameRoomUser roomUser, int row, int column, int speed, bool walk = false) {
            RoomUser = roomUser;

            Speed = speed;

            Row = row;
            Column = column;

            Walk = walk;
        }

        public GameRoomUserPosition(GameRoomUser roomUser, int row, int column, double depth, int speed, bool walk = false) {
            RoomUser = roomUser;

            Speed = speed;

            Row = row;
            Column = column;
            Depth = depth;

            Walk = walk;
        }
    }
}
