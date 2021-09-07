using System;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Rooms;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;

namespace Cortex.Server.Game.Rooms.Users.Events {
    class OnSocketClose : ISocketEvent {
        public string Event => "OnSocketClose";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("UPDATE users SET home = @home WHERE id = @id", connection)) {
                    command.Parameters.AddWithValue("@home", client.User.Room.Id);
                    command.Parameters.AddWithValue("@id", client.User.Id);

                    command.ExecuteNonQuery();
                }
            }

            GameRoomUser roomUser = client.User.Room.Users.Find(x => x.Id == client.User.Id);

            GameRoomManager.RemoveUser(client.User);

            return 1;
        }
    }
}
