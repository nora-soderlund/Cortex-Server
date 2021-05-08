using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

using Server.Game.Users;

using Server.Game.Rooms.Map;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Actions;
using Server.Game.Rooms.Furnitures;
using Server.Game.Rooms.Furnitures.Logics;
using Server.Game.Rooms.Navigator;

using Server.Socket.Messages;

namespace Server.Game.Rooms {
    class GameRoom {
        [JsonIgnore]
        public int Id;
        
        [JsonProperty("user")]
        public int User;
        
        [JsonIgnore]
        public int Access;
        
        [JsonProperty("title")]
        public string Title;
        
        [JsonProperty("description")]
        public string Description;

        [JsonIgnore]
        public List<GameRoomUser> Users = new List<GameRoomUser>();

        [JsonIgnore]
        public List<GameRoomFurniture> Furnitures = new List<GameRoomFurniture>();

        [JsonProperty("map")]
        public GameRoomMap Map;

        [JsonIgnore]
        public GameRoomActions Actions;

        [JsonProperty("rights")]
        public List<int> Rights = new List<int>();

        [JsonIgnore]
        public GameRoomNavigator Navigator;

        [JsonProperty("floor_material")]
        public string FloorMaterial;

        [JsonProperty("floor_thickness")]
        public int FloorThickness;

        [JsonProperty("wall_material")]
        public string WallMaterial;

        [JsonProperty("wall_thickness")]
        public int WallThickness;

        public GameRoom(MySqlDataReader room) {
            Id = room.GetInt32("id");
            
            User = room.GetInt32("user");

            Access = room.GetInt32("access");
            
            Title = room.GetString("title");
            
            FloorMaterial = room.GetString("floor_material");
            FloorThickness = room.GetInt32("floor_thickness");
            
            WallMaterial = room.GetString("wall_material");
            WallThickness = room.GetInt32("wall_thickness");

            Navigator = GameRoomManager.Navigator.Find(x => x.Id == Id);

            if(Navigator == null) {
                Navigator = new GameRoomNavigator(this);

                GameRoomManager.Navigator.Add(Navigator);
            }

            Navigator.Room = this;

            Description = room.GetString("description");

            Map = new GameRoomMap(this, room.GetString("map"), new GameRoomPoint(room.GetInt32("door_row"), room.GetInt32("door_column"), 0, room.GetInt32("door_direction")));

            Actions = new GameRoomActions(this);

            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            using(MySqlCommand command = new MySqlCommand("SELECT * FROM room_furnitures WHERE room = @room", connection)) {
                command.Parameters.AddWithValue("@room", Id);

                using(MySqlDataReader reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        Furnitures.Add(new GameRoomFurniture(this, reader));
                    }
                }
            }

            using(MySqlCommand command = new MySqlCommand("SELECT * FROM room_rights WHERE room = @room", connection)) {
                command.Parameters.AddWithValue("@room", Id);

                using(MySqlDataReader reader = command.ExecuteReader())
                    while(reader.Read())
                        Rights.Add(reader.GetInt32("user"));

                Rights.Add(User);
            }

            Timer.Elapsed += OnTimerElapsed;

            Timer.Start();
        }

        public void AddUser(GameUser user) {
            user.Room = this;

            GameRoomUser roomUser = new GameRoomUser(user);

            roomUser.Position = new GameRoomPoint(Map.Door.Row, Map.Door.Column, Map.GetFloorDepth(Map.Door.Row, Map.Door.Column), Map.Door.Direction);

            SocketMessage message = new SocketMessage();

            message.Add("OnRoomEntityAdd", new {
                users = roomUser
            });

            Send(message.Compose());

            Users.Add(roomUser);

            message = new SocketMessage();

            message.Add("OnRoomEnter", this);

            Dictionary<string, object> properties = new Dictionary<string, object>();

            if(Users.Count != 0)
                properties.Add("users", Users);
                
            if(Furnitures.Count != 0)
                properties.Add("furnitures", Furnitures);

            message.Add("OnRoomEntityAdd", properties);

            List<object> effects = new List<object>();

            foreach(GameRoomUser effectUser in Users.Where(x => x.Effect != 0))
                effects.Add(new {
                    id = effectUser.Id,

                    effect = effectUser.Effect
                });
            
            if(effects.Count != 0)
                message.Add("OnRoomUserEffect", effects);

            user.Client.Send(message.Compose());

            foreach(GameRoomFurniture furniture in Furnitures.Where(x => x.Logic != null))
                furniture.Logic.OnUserStreamIn(roomUser);
        }

        public void RemoveUser(GameUser user) {
            GameRoomUser roomUser = Users.FirstOrDefault(x => x.User == user);

            if(roomUser == null)
                return;

            foreach(GameRoomFurniture furniture in Furnitures.Where(x => x.Logic != null))
                furniture.Logic.OnUserStreamOut(roomUser);

            user.Room.Send(new SocketMessage("OnRoomEntityRemove", new { users = roomUser.Id }).Compose());

            foreach(GameRoomFurniture stacked in user.Room.Furnitures.FindAll(x => x.Position.Row == roomUser.Position.Row && x.Position.Column == roomUser.Position.Column)) {
                if(stacked.Logic == null)
                    continue;

                stacked.Logic.OnUserLeave(roomUser);
            }

            user.Room.Users.Remove(roomUser);

            user.Room = null;
        }

        public GameRoomUser GetUser(int id) {
            return Users.Find(x => x.Id == id);
        }

        [JsonIgnore]
        public Timer Timer = new Timer(500);

        public void OnTimerElapsed(Object source, System.Timers.ElapsedEventArgs e) {
            List<IGameRoomFurnitureIntervalLogic> logics = new List<IGameRoomFurnitureIntervalLogic>();

            foreach(GameRoomFurniture furniture in Furnitures) {
                if(furniture.Logic == null || !(furniture.Logic is IGameRoomFurnitureIntervalLogic))
                    continue;

                IGameRoomFurnitureIntervalLogic logic = furniture.Logic as IGameRoomFurnitureIntervalLogic;

                logic.IntervalCount += (int)Timer.Interval;

                if(logic.IntervalCount >= logic.Interval) {
                    logic.OnTimerPrepare();

                    logic.IntervalCount = 0;

                    logics.Add(logic);
                }
            }

            foreach(IGameRoomFurnitureIntervalLogic logic in logics)
                logic.OnTimerElapsed();

            Actions.Elapse();
        }

        public void Send(string message) {
            foreach(GameRoomUser roomUser in Users) {
                roomUser.User.Client.Send(message);
            }
        }

        public void SetTitle(string title) {
            Title = title;

            Send(new SocketMessage("OnRoomSettingsUpdate", new { title }).Compose());

            Navigator.Title = title;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE rooms SET title = @title WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", Id);
                    command.Parameters.AddWithValue("@title", Title);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void SetDescription(string description) {
            Description = description;

            Send(new SocketMessage("OnRoomSettingsUpdate", new { description }).Compose());

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE rooms SET description = @description WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@id", Id);
                    command.Parameters.AddWithValue("@description", Description);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
