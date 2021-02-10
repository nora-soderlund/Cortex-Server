using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

using Server.Game.Users;

using Server.Game.Rooms.Map;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Furnitures;
using Server.Game.Rooms.Navigator;
using Server.Game.Rooms.Navigator.Messages;

using Server.Socket.Messages;

namespace Server.Game.Rooms {
    class GameRoom {
        [JsonIgnore]
        public int Id;

        [JsonIgnore]
        public GameRoomNavigatorMessage Navigator;

        [JsonIgnore]
        public List<GameRoomUser> Users = new List<GameRoomUser>();

        [JsonIgnore]
        public List<GameRoomFurniture> Furnitures = new List<GameRoomFurniture>();

        [JsonProperty("map")]
        public GameRoomMap Map;

        [JsonIgnore]
        public GameRoomPoint Door;

        [JsonIgnore]
        public GameRoomEvents Events;

        public GameRoom(MySqlDataReader room) {
            Id = room.GetInt32("id");

            Navigator = GameRoomNavigator.Rooms.FirstOrDefault(x => x.Id == Id);

            Map = new GameRoomMap(room.GetString("map"));

            Door = new GameRoomPoint(room.GetDouble("door_row"), room.GetDouble("door_column"), Map.GetDepth(room.GetInt32("door_row"), room.GetInt32("door_column")), room.GetInt32("door_direction"));

            Events = new GameRoomEvents(this);

            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            using(MySqlCommand command = new MySqlCommand("SELECT * FROM room_furnitures WHERE room = @room", connection)) {
                command.Parameters.AddWithValue("@room", Id);

                using(MySqlDataReader reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        Furnitures.Add(new GameRoomFurniture(reader));
                    }
                }
            }
        }

        public void AddUser(GameUser user) {
            user.Room = this;

            GameRoomUser roomUser = new GameRoomUser(user);

            roomUser.Position = new GameRoomPoint(Door);

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

            user.Client.Send(message.Compose());
        }

        public void Send(string message) {
            foreach(GameRoomUser roomUser in Users) {
                roomUser.User.Client.Send(message);
            }
        }
    }
}
