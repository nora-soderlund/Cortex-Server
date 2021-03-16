using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Server.Game.Users;
using Server.Game.Users.Furnitures;
using Server.Game.Rooms;
using Server.Game.Rooms.Users;

using Server.Socket.Clients;
using Server.Socket.Events;
using Server.Socket.Messages;

namespace Server.Game.Users.Friends.Events {
    class OnUserReady : ISocketEvent {
        public string Event => "OnUserReady";

        public int Execute(SocketClient client, JToken data) {
            List<object> friends = new List<object>();

            foreach(GameUserFriend friend in client.User.Friends) {
                if(friend.Status == GameUserFriendStatus.Sent)
                    continue;

                GameUser friendUser = Game.GetUser(friend.Friend);

                if(friendUser == null)
                    continue;

                friends.Add(new {
                    id = friend.Friend,

                    status = friend.Status
                });

                GameUserFriend friendUserFriend = friendUser.Friends.Find(x => x.Friend == client.User.Id);

                if(friendUserFriend.Status != GameUserFriendStatus.Sent) {
                    friendUser.Client.Send(new SocketMessage("OnUserFriendUpdate", new {
                        id = client.User.Id,

                        status = friendUserFriend.Status
                    }).Compose());
                }
            }

            client.Send(new SocketMessage("OnUserFriendUpdate", friends).Compose());

            return 1;
        }
    }
}
