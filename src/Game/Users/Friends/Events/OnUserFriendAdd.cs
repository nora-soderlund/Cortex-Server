using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;

using Cortex.Server.Game.Users;
using Cortex.Server.Game.Users.Furnitures;
using Cortex.Server.Game.Rooms;
using Cortex.Server.Game.Rooms.Users;

using Cortex.Server.Socket.Clients;
using Cortex.Server.Socket.Events;
using Cortex.Server.Socket.Messages;

namespace Cortex.Server.Game.Users.Friends.Events {
    class OnUserFriendAdd : ISocketEvent {
        public string Event => "OnUserFriendAdd";

        public int Execute(SocketClient client, JToken data) {
            int user = data["user"].ToObject<int>();

            GameUserFriend existingFriend = client.User.Friends.Find(x => x.Friend == user);

            if(existingFriend != null) {
                if(existingFriend.Status != GameUserFriendStatus.Received)
                    return 0;

                existingFriend.SetStatus(GameUserFriendStatus.Regular);

                GameUser existingUser = Game.GetUser(existingFriend.Friend);

                if(existingUser == null) {
                    using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                        connection.Open();

                        MySqlCommand command = new MySqlCommand("UPDATE user_friends SET status = @status WHERE user = @user AND friend = @friend", connection);

                        command.Parameters.AddWithValue("@status", GameUserFriendStatus.Regular);
                        command.Parameters.AddWithValue("@user", existingFriend.Friend);
                        command.Parameters.AddWithValue("@friend", existingFriend.User);

                        command.ExecuteNonQuery();
                    }
                }
                else {
                    GameUserFriend existingUserFriend = existingUser.Friends.Find(x => x.Friend == client.User.Id);

                    existingUserFriend.SetStatus(GameUserFriendStatus.Regular);

                    existingUser.Client.Send(new SocketMessage("OnUserFriendUpdate", new {
                        id = client.User.Id,

                        status = existingUserFriend.Status
                    }).Compose());
                }

                client.Send(new SocketMessage("OnUserFriendUpdate", new {
                    id = existingFriend.Friend,

                    status = existingFriend.Status
                }).Compose());

                return 1;
            }

            GameUserFriend friend = new GameUserFriend() {
                User = client.User.Id,
                Friend = user,
                Status = GameUserFriendStatus.Sent
            };

            using(MySqlConnection connection = new MySqlConnection(Program.Connection)) {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand("INSERT INTO user_friends (user, friend, status) VALUES (@user, @friend, @status)", connection)) {
                    command.Parameters.AddWithValue("@user", client.User.Id);
                    command.Parameters.AddWithValue("@friend", user);
                    command.Parameters.AddWithValue("@status", GameUserFriendStatus.Sent);

                    command.ExecuteNonQuery();
                }

                using (MySqlCommand command = new MySqlCommand("INSERT INTO user_friends (user, friend, status) VALUES (@friend, @user, @status)", connection)) {
                    command.Parameters.AddWithValue("@user", client.User.Id);
                    command.Parameters.AddWithValue("@friend", user);
                    command.Parameters.AddWithValue("@status", GameUserFriendStatus.Received);

                    command.ExecuteNonQuery();
                }
            }

            client.User.Friends.Add(friend);

            client.Send(new SocketMessage("OnUserFriendUpdate", new {
                id = friend.Friend,

                status = friend.Status
            }).Compose());

            GameUser friendUser = Game.GetUser(friend.Friend);

            if(friendUser != null) {
                GameUserFriend friendFriend = new GameUserFriend() {
                    User = friendUser.Id,
                    Friend = client.User.Id,
                    Status = GameUserFriendStatus.Received
                };

                friendUser.Friends.Add(friendFriend);

                friendUser.Client.Send(new SocketMessage("OnUserFriendUpdate", new {
                    id = friendFriend.Friend,

                    status = friendFriend.Status
                }).Compose());
            }

            return 1;
        }
    }
}
