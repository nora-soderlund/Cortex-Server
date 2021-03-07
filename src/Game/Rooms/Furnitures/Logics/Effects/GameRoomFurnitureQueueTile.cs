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
    class GameRoomFurnitureQueueTile : IGameRoomFurnitureLogic {
        public Timer Timer = new Timer(1000);

        public List<GameRoomUser> Users = new List<GameRoomUser>();

        public GameRoomFurnitureQueueTile() {
            Timer.Elapsed += OnTimerElapsed;
        }

        public void OnTimerElapsed(Object source, ElapsedEventArgs e) {
            for(int index = 0; index < Users.Count; index++) {
                GameRoomPoint newPoint = new GameRoomPoint(Furniture.Position);
                
                newPoint.FromDirection(Furniture.Position.Direction);
                newPoint.Depth = Users[index].Position.Depth;

                Users[index].User.Room.Actions.AddEntity(Users[index].User.Id, 500, new GameRoomUserPosition(Users[index], newPoint.Row, newPoint.Column));
            }

            Timer.Stop();
        }

        public GameRoomFurniture Furniture { get; set; }

        public void OnUserUse(GameRoomUser user, JToken data) {
            
        }

        public void OnUserEnter(GameRoomUser user) {
            Users.Add(user);

            if(Timer.Enabled != true)
                Timer.Start();
        }

        public void OnUserLeave(GameRoomUser user) {
            Users.Remove(user);
        }
    }
}
