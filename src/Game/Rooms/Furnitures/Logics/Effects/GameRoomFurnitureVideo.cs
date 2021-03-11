using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Game.Furnitures;

using Server.Game.Users;
using Server.Game.Users.Furnitures;

using Server.Game.Rooms.Actions;
using Server.Game.Rooms.Users;

using Server.Game.Furnitures.Videos;

using Server.Game.Rooms.Furnitures.Actions;

using Server.Events;
using Server.Socket.Messages;

namespace Server.Game.Rooms.Furnitures.Logics {
    class GameRoomFurnitureVideo : IGameRoomFurnitureIntervalLogic {
        public GameRoomFurniture Furniture { get; set; }
        public int Interval => 1000;

        public bool Enabled = true;

        public int Video = 0;
        public int VideoTime = 0;
        public List<GameFurnitureVideo> Videos = new List<GameFurnitureVideo>();

        public GameRoomFurnitureVideo(GameRoomFurniture furniture) {
            Furniture = furniture;

            string[] videos = Furniture.Extra.Split(',');

            
            ThreadPool.QueueUserWorkItem((a) => {
                foreach(string video in videos)
                    Videos.Add(GameFurnitureManager.GetVideo(video));
            });
        }

        public void OnTimerElapsed() {
            if(!Enabled)
                return;

            if(Videos.Count == 0)
                return;

            VideoTime++;

            if(VideoTime >= Videos[Video].Length) {
                VideoTime = 0;

                Video++;

                if(Video >= Videos.Count)
                    Video = 0;
                    
                Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStart", new {
                    id = Furniture.Id,

                    link = Videos[Video].Id
                }).Compose());
            }
        }

        public void OnUserUse(GameRoomUser user, JToken data) {
            user.User.Client.Send(new SocketMessage("OnRoomFurnitureVideoUse", new {
                id = Furniture.Id,

                video = Video,

                videos = Videos,

                enabled = Enabled
            }).Compose());

            /*if(!Enabled) {
                Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStart", new {
                    id = Furniture.Id,

                    link = "http://api.project-cortex.net/YouTube/" + Furniture.Extra + ".mp4"
                }).Compose());
            }
            else
                Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStop", Furniture.Id).Compose());
        
            Enabled = !Enabled;*/
        }
    }
}
