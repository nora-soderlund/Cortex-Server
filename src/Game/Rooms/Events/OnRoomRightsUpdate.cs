using System;

using Newtonsoft.Json.Linq;

using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;

using Server.Socket.Clients;
using Server.Socket.Messages;
using Server.Socket.Events;

namespace Server.Game.Rooms.Events {
    class OnRoomRightsUpdate : ISocketEvent {
        public string Event => "OnRoomRightsUpdate";

        public int Execute(SocketClient client, JToken data) {
            if(client.User.Room == null)
                return 0;

            if(data["user"] == null) {
                client.Send(new SocketMessage("OnRoomRightsUpdate", client.User.Room.Rights).Compose());

                return 1;
            }
                
            if(client.User.Id != client.User.Room.User)
                return 0;

            int user = data["user"].ToObject<int>();

            GameRoomUser roomUser = client.User.Room.GetUser(user);

            if(roomUser == null)
                return 0;

            if(roomUser.HasRights()) {
                client.User.Room.Rights.Remove(roomUser.Id);

                using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                    connection.Open();

                    using(MySqlCommand command = new MySqlCommand("DELETE FROM room_rights WHERE room = @room AND user = @user", connection)) {
                        command.Parameters.AddWithValue("@room", client.User.Room.Id);
                        command.Parameters.AddWithValue("@user", roomUser.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }
            else {
                client.User.Room.Rights.Add(roomUser.Id);

                using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                    connection.Open();

                    using(MySqlCommand command = new MySqlCommand("INSERT INTO room_rights (room, user) VALUES (@room, @user)", connection)) {
                        command.Parameters.AddWithValue("@room", client.User.Room.Id);
                        command.Parameters.AddWithValue("@user", roomUser.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }

            SocketMessage message = new SocketMessage("OnRoomRightsUpdate", client.User.Room.Rights);

            client.Send(message.Compose());

            roomUser.User.Client.Send(message.Compose());

            return 1;
        }
    }
}
