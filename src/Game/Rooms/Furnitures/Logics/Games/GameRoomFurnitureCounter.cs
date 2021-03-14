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

using Server.Game.Rooms.Furnitures.Actions;

using Server.Events;
using Server.Socket.Messages;

namespace Server.Game.Rooms.Furnitures.Logics {
    class GameRoomFurnitureCounter : IGameRoomFurnitureIntervalLogic {
        public GameRoomFurniture Furniture { get; set; }
        
        public int Interval => 1000;
        public int IntervalCount { get; set; }

        public bool Enabled = false;

        public int Time = 0;
        public int TimeIndex = 0;

        public List<int> Times = new List<int>();

        public GameRoomFurnitureCounter(GameRoomFurniture furniture) {
            string[] parameters = furniture.UserFurniture.Furniture.Parameters.Split(',');

            foreach(string parameter in parameters)
                Times.Add(Int32.Parse(parameter));

            Time = Times[TimeIndex];

            furniture.Animation = Time;
        }

        public void OnUserUse(GameRoomUser user, JToken data) {
            string tag = data["tag"].ToString();

            if(tag == "reset") {
                if(Time == Times[TimeIndex] && Enabled != true) {
                    TimeIndex++;

                    if(TimeIndex >= Times.Count)
                        TimeIndex = 0;

                    Time = Times[TimeIndex];
                }
                else {
                    Enabled = false;

                    Time = Times[TimeIndex];
                }

                Furniture.Animation = Time;
                Furniture.Room.Actions.AddEntity(Furniture.Id, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));
            }
            else if(tag == "start_stop") {
                Enabled = !Enabled;
            
                if(Enabled) {
                    if(Time == 0)
                        Time = Times[TimeIndex];

                    foreach(GameRoomFurniture furniture in Furniture.Room.Furnitures.Where(x => x.Id != Furniture.Id))
                        furniture.Logic.OnGameStart();

                    GameRoomFurnitureBanzai.OnGameStart(Furniture);
                }
                else {
                    foreach(GameRoomFurniture furniture in Furniture.Room.Furnitures.Where(x => x.Id != Furniture.Id))
                        furniture.Logic.OnGameStop();

                    GameRoomFurnitureBanzai.OnGameStop(Furniture);
                }
            }
        }
        
        public void OnTimerElapsed() {
            if(!Enabled)
                return;

            Time--;

            if(Time < 0)
                return;

            Furniture.Animation = Time;
            Furniture.Room.Actions.AddEntity(Furniture.Id, new GameRoomFurnitureAnimation(Furniture, Furniture.Animation));

            if(Time == 0) {
                foreach(GameRoomFurniture furniture in Furniture.Room.Furnitures.Where(x => x.Id != Furniture.Id))
                    furniture.Logic.OnGameStop();

                GameRoomFurnitureBanzai.OnGameStop(Furniture);
            }
        }

        public void OnGameStop() {
            Enabled = false;
        }
    }
}
