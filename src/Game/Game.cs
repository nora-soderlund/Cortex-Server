using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Newtonsoft.Json.Linq;

using Server.Game.Users;

using Server.Game.Rooms.Map;
using Server.Game.Rooms.Users;
using Server.Game.Rooms.Actions;
using Server.Game.Rooms.Furnitures;
using Server.Game.Rooms.Navigator;

using Server.Socket;
using Server.Socket.Messages;
using Server.Socket.Events;
using Server.Socket.Clients;

namespace Server.Game {
    class Game {
        public static GameUser GetUser(long id) {
            List<SocketClient> clients = Program.Socket.clients.FindAll(x => x.User != null);

            SocketClient client = clients.Find(x => x.User.Id == id);

            if(client != null)
                return client.User;

            return null;
        }

        class OnUserRequest : ISocketEvent {
            public string Event => "OnUserRequest";

            public int Execute(SocketClient client, JToken data) {
                int id = data.ToObject<int>();

                GameUser user = GetUser(id);

                if(user != null) {
                    client.Send(new SocketMessage("OnUserRequest", new {
                        id = id,

                        name = user.Name
                    }).Compose());

                    return 1;
                }

                using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                    connection.Open();

                    using(MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE id = @id", connection)) {
                        command.Parameters.AddWithValue("@id", id);

                        MySqlDataReader reader = command.ExecuteReader();

                        if(!reader.Read())
                            return 0;

                        client.Send(new SocketMessage("OnUserRequest", new {
                            id = id,

                            name = reader.GetString("name")
                        }).Compose());
                    }
                }
            
                return 1;
            }
        }
    }
}
