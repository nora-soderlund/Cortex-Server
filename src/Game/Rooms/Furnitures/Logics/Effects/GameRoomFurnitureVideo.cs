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
        public int IntervalCount { get; set; }

        public bool Enabled = false;

        public int Video = 0;
        public int VideoTime = 0;
        public List<GameFurnitureVideo> Videos = new List<GameFurnitureVideo>();

        public GameRoomFurnitureVideo(GameRoomFurniture furniture) {
            Furniture = furniture;

            if(Furniture.Extra == null || Furniture.Extra.Length == 0)
                return;

            Enabled = true;

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

                    video = Videos[Video].Id
                }).Compose());
            }
        }

        public void OnUserStreamIn(GameRoomUser user) {
            if(!Enabled)
                return;

            if(Videos.Count == 0)
                return;

            user.User.Client.Send(new SocketMessage("OnRoomFurnitureVideoStart", new {
                id = Furniture.Id,

                video = Videos[Video].Id,
                time = VideoTime
            }).Compose());
        }

        public void OnUserUse(GameRoomUser user, JToken data) {
            if(!user.HasRights())
                return;

            if(data["action"] != null) {
                string action = data["action"].ToString();

                if(data["video"] == null) {
                    if(Videos.Count == 0) {
                        user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", false).Compose());

                        return;
                    }

                    if(action == "next") {
                        if(Videos.Count < 2) {
                            user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", false).Compose());

                            return;
                        }

                        Video++;

                        VideoTime = 0;

                        if(Video >= Videos.Count)
                            Video = 0;

                        Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStart", new {
                            id = Furniture.Id,

                            video = Videos[Video].Id,
                            time = VideoTime
                        }).Compose());

                        user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", true).Compose());
                    }
                    else if(action == "previous") {
                        if(Videos.Count < 2) {
                            user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", false).Compose());

                            return;
                        }

                        Video--;

                        VideoTime = 0;

                        if(Video == -1)
                            Video = Videos.Count - 1;

                        Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStart", new {
                            id = Furniture.Id,

                            video = Videos[Video].Id,
                            time = VideoTime
                        }).Compose());

                        user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", true).Compose());
                    }
                    else if(action == "stop") {
                        user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", false).Compose());

                        Enabled = false;

                        Video = 0;
                        VideoTime = 0;

                        Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStop", Furniture.Id).Compose());

                        user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", true).Compose());
                    }
                    else if(action == "pause") {
                        user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", false).Compose());

                        if(!Enabled) {
                            Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStart", new {
                                id = Furniture.Id,

                                video = Videos[Video].Id,
                                time = VideoTime
                            }).Compose());
                        }
                        else
                            Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStop", Furniture.Id).Compose());

                        Enabled = !Enabled;

                        user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", true).Compose());
                    }
                }
                else {
                    string video = data["video"].ToString();

                    if(action == "add") {
                        try {
                            GameFurnitureVideo furnitureVideo = GameFurnitureManager.GetVideo(video);

                            if(Videos.Find(x => x.Id == furnitureVideo.Id) != null) {
                                user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", false).Compose());

                                return;
                            }

                            Videos.Add(furnitureVideo);

                            Enabled = true;

                            Video = Videos.IndexOf(furnitureVideo);
                            VideoTime = 0;

                            Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStart", new {
                                id = Furniture.Id,

                                video = Videos[Video].Id,
                                time = VideoTime
                            }).Compose());

                            UpdateFurnitureExtra(); 

                            user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", true).Compose());
                        }
                        catch(Exception exception) {
                            Console.Exception(exception);

                            user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", false).Compose());
                        }
                    }
                    else if(action == "remove") {
                        GameFurnitureVideo furnitureVideo = Videos.Find(x => x.Id == video);

                        if(furnitureVideo == null)
                            return;

                        int index = Videos.IndexOf(furnitureVideo);

                        Videos.Remove(furnitureVideo);

                        if(Video == index) {
                            VideoTime = 0;

                            if(Videos.Count != 0) {
                                Video++;

                                if(Video >= Videos.Count)
                                    Video = 0;

                                user.User.Client.Send(new SocketMessage("OnRoomFurnitureVideoStart", new {
                                    id = Furniture.Id,

                                    video = Videos[Video].Id
                                }).Compose());
                            }
                            else
                                Furniture.Room.Send(new SocketMessage("OnRoomFurnitureVideoStop", Furniture.Id).Compose());
                        }

                        UpdateFurnitureExtra();

                        user.User.Client.Send(new SocketMessage("OnRoomFurnitureUse", true).Compose());
                    }
                }

                return;
            }

            user.User.Client.Send(new SocketMessage("OnRoomFurnitureVideoUse", new {
                id = Furniture.Id,

                video = Video,

                videos = Videos,

                enabled = Enabled
            }).Compose());
        }

        public void UpdateFurnitureExtra() {
            Furniture.Extra = "";

            foreach(GameFurnitureVideo video in Videos) {
                if(Furniture.Extra.Length != 0)
                    Furniture.Extra += ",";

                Furniture.Extra += video.Id;
            }

            using MySqlConnection connection = new MySqlConnection(Program.Connection);
            connection.Open();

            using MySqlCommand command = new MySqlCommand("UPDATE room_furnitures SET extra = @extra WHERE id = @id", connection);
            command.Parameters.AddWithValue("@extra", Furniture.Extra);
            command.Parameters.AddWithValue("@id", Furniture.Id);

            command.ExecuteNonQuery();
        }
    }
}
