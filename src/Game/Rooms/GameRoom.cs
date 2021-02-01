using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

using Server.Game.Users;

using Server.Game.Rooms.Map;
using Server.Game.Rooms.Users;
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

        [JsonProperty("map")]
        public GameRoomMap Map;

        [JsonIgnore]
        public GameRoomPoint Door;

        [JsonIgnore]
        public GameRoomEvents Events;

        public GameRoom(MySqlDataReader reader) {
            Id = reader.GetInt32("id");

            Navigator = GameRoomNavigator.Rooms.FirstOrDefault(x => x.Id == Id);

            Map = new GameRoomMap(reader.GetString("map"));

            Door = new GameRoomPoint(reader.GetDouble("door_row"), reader.GetDouble("door_column"), 0, reader.GetInt32("door_direction"));

            Events = new GameRoomEvents(this);
        }

        public void AddUser(GameUser user) {
            user.Room = this;

            GameRoomUser roomUser = new GameRoomUser(user);

            roomUser.Position = new GameRoomPoint(Door);

            Users.Add(roomUser);

            SocketMessage message = new SocketMessage();

            message.Add("OnRoomEnter", this);

            message.Add("OnRoomEntityAdd", new {
                users = Users
            });

            user.Client.Send(message.Compose());
        }

        public void Send(string message) {
            foreach(GameRoomUser roomUser in Users) {
                roomUser.User.Client.Send(message);
            }
        }
    }
}
