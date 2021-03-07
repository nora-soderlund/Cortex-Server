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

        public List<GameRoomUser> PendingUsers = new List<GameRoomUser>();
        public List<GameRoomFurniture> PendingFurnitures = new List<GameRoomFurniture>();

        public List<GameRoomUser> Users = new List<GameRoomUser>();
        public List<GameRoomFurniture> Furnitures = new List<GameRoomFurniture>();

        public int Interval => 3000;
        public int IntervalCount { get; set; }

        public void OnTimerPrepare() {
            Users.Clear();
            Furnitures.Clear();

            Users = new List<GameRoomUser>(PendingUsers);
            Furnitures = new List<GameRoomFurniture>(PendingFurnitures);

            PendingUsers.Clear();
            PendingFurnitures.Clear();
        }

        public void OnTimerElapsed() {
            GameRoomPoint newPoint = new GameRoomPoint(Furniture.Position);
            newPoint.FromDirection(Furniture.Position.Direction);

            foreach(GameRoomFurniture furniture in Furnitures) {
                if(!furniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Sitable) && !furniture.UserFurniture.Furniture.Flags.HasFlag(GameFurnitureFlags.Standable)) {
                    if(furniture.Room.Users.Find(x => x.Position.Row == newPoint.Row && x.Position.Column == newPoint.Column) != null)
                        continue;
                }

                furniture.Room.Actions.AddEntity(furniture.Id, 500, new GameRoomFurniturePosition(furniture, new GameRoomPoint(newPoint.Row, newPoint.Column, furniture.Position.Depth, furniture.Position.Direction), 500));
            }

            foreach(GameRoomUser user in Users)
                user.User.Room.Actions.AddEntity(user.User.Id, 500, new GameRoomUserPosition(user, newPoint.Row, newPoint.Column, 500, false));
        }

        public void OnUserUse(GameRoomUser user, JToken data) {
            
        }

        public void OnUserEnter(GameRoomUser user) {
            PendingUsers.Add(user);
        }

        public void OnUserLeave(GameRoomUser user) {
            PendingUsers.Remove(user);
        }

        public void OnFurnitureEnter(GameRoomFurniture furniture) {
            PendingFurnitures.Add(furniture);
        }

        public void OnFurnitureLeave(GameRoomFurniture furniture) {
            PendingFurnitures.Remove(furniture);
        }
    }
}
