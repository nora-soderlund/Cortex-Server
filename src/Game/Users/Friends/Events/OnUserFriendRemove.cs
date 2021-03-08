using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;

using Server.Game.Users;
using Server.Game.Users.Furnitures;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Users.Friends.Events {
    class OnUserFriendRemove : ISocketEvent {
        public string Event => "OnUserFriendRemove";

        public int Execute(SocketClient client, JToken data) {
            int id = data["user"].ToObject<int>();

            GameUserFriend friend = client.User.Friends.Find(x => x.Friend == id);

            if(friend == null)
                return 0;

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand("DELETE FROM user_friends WHERE user = @user AND friend = @friend", connection)) {
                    command.Parameters.AddWithValue("@user", client.User.Id);
                    command.Parameters.AddWithValue("@friend", friend.Friend);

                    command.ExecuteNonQuery();
                }

                using(MySqlCommand command = new MySqlCommand("DELETE FROM user_friends WHERE user = @friend AND friend = @user", connection)) {
                    command.Parameters.AddWithValue("@user", client.User.Id);
                    command.Parameters.AddWithValue("@friend", friend.Friend);

                    command.ExecuteNonQuery();
                }
            }

            client.User.Friends.Remove(friend);

            client.Send(new SocketMessage("OnUserFriendRemove", friend.Friend).Compose());

            GameUser friendUser = Game.GetUser(friend.Friend);

            if(friendUser != null) {
                friendUser.Friends.Remove(friendUser.Friends.Find(x => x.Friend == client.User.Id));

                friendUser.Client.Send(new SocketMessage("OnUserFriendRemove", client.User.Id).Compose());
            }

            return 1;
        }
    }
}
