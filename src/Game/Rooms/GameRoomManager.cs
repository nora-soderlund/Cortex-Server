using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Users;

using Server.Socket.Messages;

namespace Server.Game.Rooms {
    class GameRoomManager {
        public static List<GameRoom> Rooms = new List<GameRoom>();

        public static GameRoom Load(long id) {
            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            using MySqlCommand command = new MySqlCommand("SELECT * FROM rooms WHERE id = @id", connection);

            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if(!reader.Read())
                return null;

            GameRoom room = new GameRoom(reader);

            Rooms.Add(room);

            return room;
        }

        public static void AddUser(long id, GameUser user) {
            if(user.Room != null && user.Room.Id == id)
                return;

            RemoveUser(user);

            GameRoom room = Rooms.FirstOrDefault(x => x.Id == id);

            if(room == null && (room = Load(id)) == null)
                return;

            room.AddUser(user);

            if(user.Room.Navigator != null)
                user.Room.Navigator.UpdateUsers(user.Room.Users.Count());
        }

        public static void RemoveUser(GameUser user) {
            if(user.Room == null)
                return;

            GameRoomUser roomUser = user.Room.Users.FirstOrDefault(x => x.User == user);

            if(roomUser == null)
                return;

            user.Room.Users.Remove(roomUser);

            user.Room.Navigator.UpdateUsers(user.Room.Users.Count());

            user.Room = null;
        }
    }
}
