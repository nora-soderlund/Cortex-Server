using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms.Users;

using Server.Socket.Messages;
using Server.Events;

using Server.Game.Rooms.Navigator;

namespace Server.Game.Rooms {
    class GameRoomManager : IInitializationEvent {

        public static List<GameRoom> Rooms = new List<GameRoom>();
        public static List<GameRoomNavigator> Navigator = new List<GameRoomNavigator>();

        public void OnInitialization() {
            using MySqlConnection connection = new MySqlConnection(Program.Connection);

            connection.Open();

            using MySqlCommand command = new MySqlCommand("SELECT * FROM rooms", connection);

            using MySqlDataReader reader = command.ExecuteReader();

            while(reader.Read())
                Navigator.Add(new GameRoomNavigator(reader));

            Program.WriteLine("Read " + Rooms.Count + " rooms to the navigator memory...");
        }

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
        }

        public static void RemoveUser(GameUser user) {
            if(user.Room == null)
                return;

            user.Room.RemoveUser(user);
        }
    }
}
