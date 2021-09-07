using System;
using System.Linq;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

using Newtonsoft.Json.Linq;

using Cortex.Server.Game.Users;

using Cortex.Server.Game.Rooms.Map;
using Cortex.Server.Game.Rooms.Users;
using Cortex.Server.Game.Rooms.Actions;
using Cortex.Server.Game.Rooms.Furnitures;
using Cortex.Server.Game.Rooms.Navigator;

using Cortex.Server.Socket;
using Cortex.Server.Socket.Messages;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Clients;

namespace Cortex.Server.Game {
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

                        name = user.Name,
                        figure = user.Figure
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

                            name = reader.GetString("name"),
                            figure = reader.GetString("figure")
                        }).Compose());
                    }
                }
            
                return 1;
            }
        }
    }
}
