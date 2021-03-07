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
        public int IntervalCount { get; set; }

        public void OnTimerElapsed() {
            GameRoomPoint newPoint = new GameRoomPoint(Furniture.Position);
            newPoint.FromDirection(Furniture.Position.Direction);

            for(int index = 0; index < Users.Count; index++)
                Users[index].User.Room.Actions.AddEntity(Users[index].User.Id, 500, new GameRoomUserPosition(Users[index], newPoint.Row, newPoint.Column));

            for(int index = 0; index < Furnitures.Count; index++)
                Furnitures[index].Room.Actions.AddEntity(Furnitures[index].Id, 500, new GameRoomFurniturePosition(Furnitures[index], new GameRoomPoint(newPoint.Row, newPoint.Column, Furnitures[index].Position.Depth, Furnitures[index].Position.Direction), 500));
        }

        public void OnUserUse(GameRoomUser user, JToken data) {
            
        }

        public void OnUserEnter(GameRoomUser user) {
            Users.Add(user);
        }

        public void OnUserLeave(GameRoomUser user) {
            Users.Remove(user);
        }

        public void OnFurnitureEnter(GameRoomFurniture furniture) {
            Furnitures.Add(furniture);
        }

        public void OnFurnitureLeave(GameRoomFurniture furniture) {
            Furnitures.Remove(furniture);
        }
    }
}
