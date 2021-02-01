using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Events;

using Server.Database;

using Server.Socket.Clients;
using Server.Socket.Messages;
using Server.Socket.Events;

using Server.Game.Rooms.Navigator.Messages;

namespace Server.Game.Rooms.Navigator {
    class GameRoomNavigator : IInitializationEvent {
        public static List<GameRoomNavigatorMessage> Rooms = new List<GameRoomNavigatorMessage>();

        public void OnInitialization() {
            using MySqlCommand command = new MySqlCommand("SELECT * FROM rooms", Program.Database.Connection);

            using MySqlDataReader reader = command.ExecuteReader();

            while(reader.Read())
                Rooms.Add(new GameRoomNavigatorMessage(reader));
        }

        public static void UpdateRooms() {
            Rooms = Rooms.OrderByDescending(x => x.Users).ToList();
        }

        class OnRoomNavigatorUpdate : ISocketEvent {
            public string Event => "OnRoomNavigatorUpdate";

            public int Execute(SocketClient client, JToken data) {
                switch(data.ToString()) {
                    case "my_rooms": {
                        client.Send(new SocketMessage("OnRoomNavigatorUpdate", new {
                            owned = Rooms.Where(x => x.Owner == client.User.Id).ToList()
                        }).Compose());

                        return 1;
                    }
                }

                return 0;
            }
        }
    }
}
