using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Game.Furnitures;

using Server.Game.Users;
using Server.Game.Users.Furnitures;

using Server.Game.Rooms.Actions;

using Server.Game.Rooms.Users;
using Server.Game.Rooms.Users.Actions;

using Server.Game.Rooms.Furnitures.Actions;

using Server.Events;
using Server.Socket.Messages;

namespace Server.Game.Rooms.Furnitures.Logics {
    class GameRoomFurnitureQueueTile : IGameRoomFurnitureIntervalLogic {
        public GameRoomFurniture Furniture { get; set; }

        public List<GameRoomUser> Users = new List<GameRoomUser>();
        public List<GameRoomFurniture> Furnitures = new List<GameRoomFurniture>();

        public int Interval => 3000;

        public void OnTimerPrepare() {
            Users.Clear();
            
            foreach(GameRoomUser user in Furniture.Room.Users.Where(x => (x.Position.Row == Furniture.Position.Row) && (x.Position.Column == Furniture.Position.Column)))
                Users.Add(user);
                
            Furnitures.Clear();
            
            foreach(GameRoomFurniture furniture in Furniture.Room.Furnitures.Where(x => (x.Position.Row == Furniture.Position.Row) && (x.Position.Column == Furniture.Position.Column) && (x.Position.Depth >= Furniture.Position.Depth) && (x.Id != Furniture.Id)))
                Furnitures.Add(furniture);
        }

        public void OnTimerElapsed() {
            GameRoomPoint newPoint = new GameRoomPoint(Furniture.Position);

            newPoint.FromDirection(Furniture.Position.Direction);

            if(Furniture.Room.Users.Find(x => x.Position.Row == newPoint.Row && x.Position.Column == newPoint.Column) != null)
                return;

            GameRoomFurniture nextRoller = Furniture.Room.Furnitures.Find(x => (x.Logic is GameRoomFurnitureQueueTile) && (x.Position.Row == newPoint.Row) && (x.Position.Column == newPoint.Column) && (x.Id != Furniture.Id));

            newPoint.Depth = (nextRoller == null)?(Furniture.GetDimension().Depth):(0);

            foreach(GameRoomFurniture furniture in Furnitures) {
                if(!furniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Sitable) && !furniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Standable)) {
                    if(furniture.Room.Users.Find(x => x.Position.Row == newPoint.Row && x.Position.Column == newPoint.Column) != null)
                        continue;
                }

                furniture.Room.Actions.AddEntity(furniture.Id, 500, new GameRoomFurniturePosition(furniture, new GameRoomPoint(newPoint.Row, newPoint.Column, furniture.Position.Depth - newPoint.Depth, furniture.Position.Direction), 500));
            }

            foreach(GameRoomUser user in Users)
                user.User.Room.Actions.AddEntity(user.User.Id, 500, new GameRoomUserPosition(user, newPoint.Row, newPoint.Column, 500, false));
        }
    }
}
